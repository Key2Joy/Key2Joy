using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Key2Joy.LowLevelInput;

/// <summary>
/// Reads relative mouse movement through Raw Input: what the mouse reports, without
/// Windows pointer acceleration, pointer speed or screen edges. Raw Input also keeps
/// arriving while a low-level mouse hook blocks the movement.
///
/// Runs its own thread with a hidden message-only window. <see cref="MouseMoved"/> is
/// raised on that thread.
/// </summary>
public sealed class RawMouseInput : IDisposable
{
    private const uint WM_INPUT = 0x00FF;
    private const uint WM_QUIT = 0x0012;
    private const uint RIDEV_INPUTSINK = 0x00000100;
    private const uint RIDEV_REMOVE = 0x00000001;
    private const uint RID_INPUT = 0x10000003;
    private const uint RIM_TYPEMOUSE = 0;
    private const ushort MOUSE_MOVE_ABSOLUTE = 0x01;
    private const ushort HID_USAGE_PAGE_GENERIC = 0x01;
    private const ushort HID_USAGE_GENERIC_MOUSE = 0x02;
    private static readonly IntPtr HWND_MESSAGE = new(-3);

    /// <summary>
    /// Raised with the relative movement (in mouse counts) of each raw mouse event.
    /// Raised on the Raw Input thread.
    /// </summary>
    public event Action<int, int> MouseMoved;

    private readonly Thread thread;
    private readonly ManualResetEventSlim started = new(false);
    private uint threadId;
    private Exception startException;
    private bool isDisposed;

    public RawMouseInput()
    {
        this.thread = new Thread(this.Run)
        {
            IsBackground = true,
            Name = "Key2Joy Raw Mouse Input",
            Priority = ThreadPriority.AboveNormal,
        };
        this.thread.SetApartmentState(ApartmentState.STA);
        this.thread.Start();

        this.started.Wait();

        if (this.startException != null)
        {
            throw this.startException;
        }
    }

    private void Run()
    {
        IntPtr hwnd;

        try
        {
            this.threadId = GetCurrentThreadId();

            hwnd = CreateWindowEx(0, "STATIC", "Key2Joy Raw Mouse Input", 0, 0, 0, 0, 0, HWND_MESSAGE, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            if (hwnd == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to create the raw mouse input window.");
            }

            if (!RegisterMouse(RIDEV_INPUTSINK, hwnd))
            {
                var errorCode = Marshal.GetLastWin32Error();
                DestroyWindow(hwnd);
                throw new Win32Exception(errorCode, "Failed to register for raw mouse input.");
            }
        }
        catch (Exception ex)
        {
            this.startException = ex;
            this.started.Set();
            return;
        }

        this.started.Set();

        var headerSize = (uint)Marshal.SizeOf<RAWINPUTHEADER>();
        var bufferSize = 256u;
        var buffer = Marshal.AllocHGlobal((int)bufferSize);

        try
        {
            while (GetMessage(out var msg, IntPtr.Zero, 0, 0) > 0)
            {
                if (msg.message == WM_INPUT)
                {
                    this.HandleRawInput(msg.lParam, ref buffer, ref bufferSize, headerSize);
                }

                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }
        finally
        {
            RegisterMouse(RIDEV_REMOVE, IntPtr.Zero);
            DestroyWindow(hwnd);
            Marshal.FreeHGlobal(buffer);
        }
    }

    private void HandleRawInput(IntPtr rawInputHandle, ref IntPtr buffer, ref uint bufferSize, uint headerSize)
    {
        var size = 0u;

        if (GetRawInputData(rawInputHandle, RID_INPUT, IntPtr.Zero, ref size, headerSize) != 0)
        {
            // Querying the size returns 0 on success
            return;
        }

        if (size > bufferSize)
        {
            buffer = Marshal.ReAllocHGlobal(buffer, (IntPtr)size);
            bufferSize = size;
        }

        size = bufferSize;

        if (GetRawInputData(rawInputHandle, RID_INPUT, buffer, ref size, headerSize) == unchecked((uint)-1))
        {
            return;
        }

        var header = Marshal.PtrToStructure<RAWINPUTHEADER>(buffer);

        if (header.dwType != RIM_TYPEMOUSE)
        {
            return;
        }

        // RAWMOUSE: usFlags (2 bytes, padded to 4), button data (4), ulRawButtons (4), lLastX (4), lLastY (4)
        var flags = (ushort)Marshal.ReadInt16(buffer, (int)headerSize);

        if ((flags & MOUSE_MOVE_ABSOLUTE) != 0)
        {
            // Absolute positions (e.g. from tablets or remote desktop) aren't relative movement
            return;
        }

        var lastX = Marshal.ReadInt32(buffer, (int)headerSize + 12);
        var lastY = Marshal.ReadInt32(buffer, (int)headerSize + 16);

        if (lastX == 0 && lastY == 0)
        {
            // Button or wheel only
            return;
        }

        MouseMoved?.Invoke(lastX, lastY);
    }

    private static bool RegisterMouse(uint flags, IntPtr hwnd)
    {
        var device = new RAWINPUTDEVICE
        {
            usUsagePage = HID_USAGE_PAGE_GENERIC,
            usUsage = HID_USAGE_GENERIC_MOUSE,
            dwFlags = flags,
            hwndTarget = hwnd,
        };

        return RegisterRawInputDevices([device], 1, (uint)Marshal.SizeOf<RAWINPUTDEVICE>());
    }

    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.isDisposed = true;
        MouseMoved = null;

        if (this.startException == null)
        {
            PostThreadMessage(this.threadId, WM_QUIT, IntPtr.Zero, IntPtr.Zero);
            this.thread.Join(TimeSpan.FromSeconds(2));
        }

        this.started.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RAWINPUTDEVICE
    {
        public ushort usUsagePage;
        public ushort usUsage;
        public uint dwFlags;
        public IntPtr hwndTarget;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RAWINPUTHEADER
    {
        public uint dwType;
        public uint dwSize;
        public IntPtr hDevice;
        public IntPtr wParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public Point pt;
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateWindowEx(uint exStyle, string className, string windowName, uint style, int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(IntPtr hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] devices, uint count, uint size);

    [DllImport("user32.dll")]
    private static extern uint GetRawInputData(IntPtr rawInput, uint command, IntPtr data, ref uint size, uint headerSize);

    [DllImport("user32.dll")]
    private static extern int GetMessage(out MSG msg, IntPtr hwnd, uint filterMin, uint filterMax);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TranslateMessage(ref MSG msg);

    [DllImport("user32.dll")]
    private static extern IntPtr DispatchMessage(ref MSG msg);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PostThreadMessage(uint threadId, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();
}
