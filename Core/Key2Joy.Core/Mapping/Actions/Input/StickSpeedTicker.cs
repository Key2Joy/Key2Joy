using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Key2Joy.Mapping.Actions.Input;

/// <summary>
/// Something that needs regular updates while it has speed to apply to a stick.
/// </summary>
internal interface IStickSpeedTickTarget
{
    /// <summary>
    /// Whether the target still needs ticks (e.g. it has movement in its window, or its
    /// stick isn't back at the center yet).
    /// </summary>
    bool HasPendingSpeedWork { get; }

    /// <summary>
    /// Called on every tick while the target is active.
    /// </summary>
    /// <param name="now">The <see cref="Stopwatch"/> timestamp of the tick</param>
    void OnSpeedTick(long now);
}

/// <summary>
/// Ticks active <see cref="IStickSpeedTickTarget"/>s every few milliseconds on a single
/// background thread, which sleeps while there are none. Uses a high resolution timer,
/// because regular .NET timers only tick about every 15.6 ms on Windows, which is
/// roughly a whole game frame.
/// </summary>
internal static class StickSpeedTicker
{
    /// <summary>
    /// Time between ticks, in 100 ns units (4 ms, 250 updates per second)
    /// </summary>
    private const long TICK_INTERVAL_100NS = 40_000;

    private static readonly object syncRoot = new();
    private static readonly HashSet<IStickSpeedTickTarget> activeTargets = new();
    private static readonly AutoResetEvent wakeUp = new(false);
    private static Thread thread;

    /// <summary>
    /// Makes sure the target is ticked until it has no more pending work.
    /// </summary>
    public static void Activate(IStickSpeedTickTarget target)
    {
        lock (syncRoot)
        {
            activeTargets.Add(target);

            if (thread == null)
            {
                thread = new Thread(Run)
                {
                    IsBackground = true,
                    Name = "Key2Joy Stick Speed Ticker",
                    Priority = ThreadPriority.AboveNormal,
                };
                thread.Start();
            }
        }

        wakeUp.Set();
    }

    /// <summary>
    /// Stops ticking the target.
    /// </summary>
    public static void Deactivate(IStickSpeedTickTarget target)
    {
        lock (syncRoot)
        {
            activeTargets.Remove(target);
        }
    }

    private static void Run()
    {
        using var timer = new WaitableTimer();
        var snapshot = new List<IStickSpeedTickTarget>();

        while (true)
        {
            lock (syncRoot)
            {
                snapshot.Clear();
                snapshot.AddRange(activeTargets);
            }

            if (snapshot.Count == 0)
            {
                wakeUp.WaitOne();
                continue;
            }

            var now = Stopwatch.GetTimestamp();

            foreach (var target in snapshot)
            {
                try
                {
                    target.OnSpeedTick(now);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Stick speed tick failed: {ex}");
                }
            }

            // Checked under the lock, so a target that gets new movement right after being
            // found idle is re-added by Activate, instead of being dropped.
            lock (syncRoot)
            {
                activeTargets.RemoveWhere(target => !target.HasPendingSpeedWork);
            }

            timer.Wait(TICK_INTERVAL_100NS);
        }
    }

    private sealed class WaitableTimer : IDisposable
    {
        private const uint CREATE_WAITABLE_TIMER_HIGH_RESOLUTION = 0x00000002;
        private const uint TIMER_ALL_ACCESS = 0x001F0003;
        private const uint INFINITE = 0xFFFFFFFF;
        private const uint WAIT_OBJECT_0 = 0;

        private readonly IntPtr handle;

        public WaitableTimer()
        {
            this.handle = CreateWaitableTimerExW(IntPtr.Zero, null, CREATE_WAITABLE_TIMER_HIGH_RESOLUTION, TIMER_ALL_ACCESS);

            if (this.handle == IntPtr.Zero)
            {
                // High resolution timers need Windows 10 1803 or later
                this.handle = CreateWaitableTimerExW(IntPtr.Zero, null, 0, TIMER_ALL_ACCESS);
            }
        }

        public void Wait(long interval100ns)
        {
            if (this.handle == IntPtr.Zero)
            {
                Thread.Sleep(TimeSpan.FromTicks(interval100ns));
                return;
            }

            // Negative means relative to now
            var dueTime = -interval100ns;

            if (!SetWaitableTimer(this.handle, ref dueTime, 0, IntPtr.Zero, IntPtr.Zero, false))
            {
                Thread.Sleep(TimeSpan.FromTicks(interval100ns));
                return;
            }

            if (WaitForSingleObject(this.handle, INFINITE) != WAIT_OBJECT_0)
            {
                // Don't spin if waiting fails
                Thread.Sleep(TimeSpan.FromTicks(interval100ns));
            }
        }

        public void Dispose()
        {
            if (this.handle != IntPtr.Zero)
            {
                CloseHandle(this.handle);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateWaitableTimerExW(IntPtr lpTimerAttributes, string lpTimerName, uint dwFlags, uint dwDesiredAccess);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWaitableTimer(IntPtr hTimer, ref long pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutine, [MarshalAs(UnmanagedType.Bool)] bool fResume);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
    }
}
