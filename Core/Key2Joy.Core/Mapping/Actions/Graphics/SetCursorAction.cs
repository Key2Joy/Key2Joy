using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Contracts.Mapping.Triggers;

namespace Key2Joy.Mapping.Actions.Graphics;

[Action(
    Description = "Changes the system cursor",
    NameFormat = "Set system cursor to '{0}'",
    GroupName = "Graphics"
)]
public class SetCursorAction : CoreAction
{
    [DllImport("user32.dll")] private static extern IntPtr CreateCursor(IntPtr hInst, int xHotSpot, int yHotSpot, int nWidth, int nHeight, byte[] pvANDPlane, byte[] pvXORPlane);
    [DllImport("user32.dll")] private static extern bool SetSystemCursor(IntPtr hcur, uint id);
    [DllImport("user32.dll")] private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);
    [DllImport("user32.dll")] private static extern IntPtr LoadImage(IntPtr hInst, string name, uint type, int cx, int cy, uint fuLoad);
    [DllImport("user32.dll")] private static extern IntPtr CopyImage(IntPtr hImage, uint uType, int cx, int cy, uint fuFlags);
    [DllImport("user32.dll")] private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

    private const uint SPI_SETCURSORS = 0x0057;
    private const uint IMAGE_CURSOR = 2;
    private const uint IMAGE_ICON = 1;
    private const uint LR_LOADFROMFILE = 0x0010;
    private const uint LR_LOADFROMFILE_SHARED = 0x8010;

    public static readonly Dictionary<string, uint> SystemCursorIds = new()
    {
        { "APPSTARTING", 32650 },
        { "ARROW",       32512 },
        { "CROSS",       32515 },
        { "HAND",        32649 },
        { "HELP",        32651 },
        { "IBEAM",       32513 },
        { "NO",          32648 },
        { "SIZEALL",     32646 },
        { "SIZENESW",    32643 },
        { "SIZENS",      32645 },
        { "SIZENWSE",    32642 },
        { "SIZEWE",      32644 },
        { "UPARROW",     32516 },
        { "WAIT",        32514 },

        //
        // Special internal IDs:
        //

        // Used to reset the cursor to the default arrow, allowing the system
        // to take back control of cursor display and automatically switch between different cursor types as needed.
        { "RESET",       0 },

        // Used to hide the cursor entirely by replacing it with a transparent bitmap.
        { "HIDDEN",      1 },

        // Used to indicate a custom cursor file path, which is handled separately from the built-in named cursors.
        { "FILE",        2 },
    };

    /// <summary>
    /// Optional path to a .cur, .ani, or .ico file. Leave empty to use a named cursor or hide the cursor.
    /// </summary>
    [JsonInclude]
    public string CursorFilePath { get; set; } = string.Empty;

    /// <summary>
    /// A built-in Windows cursor name (e.g. "Arrow", "Hand", "Wait").
    /// Ignored when CursorFilePath is set. Leave empty to hide the cursor.
    /// </summary>
    [JsonInclude]
    public string CursorName { get; set; } = string.Empty;

    /// <summary>
    /// Desired cursor width in pixels. 0 = use the system default size.
    /// </summary>
    [JsonInclude]
    public int Width { get; set; } = 0;

    /// <summary>
    /// Desired cursor height in pixels. 0 = use the system default size.
    /// </summary>
    [JsonInclude]
    public int Height { get; set; } = 0;

    public SetCursorAction(string name)
        : base(name)
    { }

    /// <markdown-doc>
    /// <parent-name>Logic</parent-name>
    /// <path>Api/Logic</path>
    /// </markdown-doc>
    /// <summary>
    /// Changes every system cursor.
    ///
    /// - Pass no arguments (or an empty string) to **hide** the cursor entirely.
    /// - Pass a built-in cursor name such as `"Arrow"`, `"Hand"` or `"Wait"` to replace all cursors with that shape.
    /// - Pass a file path to a `.cur`, `.ani`, or `.ico` file to use a custom cursor image.
    ///
    /// Call `Cursor.Restore` to undo any changes made by this method.
    /// </summary>
    /// <markdown-example>
    /// Replaces every cursor with the built-in Wait (hourglass) cursor.
    /// <code language="lua">
    /// <![CDATA[
    /// Cursor.Set("Wait")
    /// ]]>
    /// </code>
    /// </markdown-example>
    /// <markdown-example>
    /// Loads a custom cursor from disk.
    /// <code language="lua">
    /// <![CDATA[
    /// Cursor.Set("C:\\cursors\\my_cursor.ani")
    /// ]]>
    /// </code>
    /// </markdown-example>
    /// <name>Cursor.Set</name>
    /// <param name="cursorOrPath">
    /// Built-in cursor name, path to a .cur/.ani/.ico file, or empty to hide the cursor.
    /// </param>
    /// <param name="width">Width in pixels (0 = system default).</param>
    /// <param name="height">Height in pixels (0 = system default).</param>
    [ExposesScriptingMethod("Cursor.Set")]
    public void ExecuteForScript(string cursorOrPath = "", int width = 0, int height = 0)
    {
        this.CursorFilePath = string.Empty;
        this.CursorName = string.Empty;
        this.Width = width;
        this.Height = height;

        if (!string.IsNullOrEmpty(cursorOrPath) && File.Exists(cursorOrPath))
        {
            this.CursorFilePath = cursorOrPath;
        }
        else
        {
            this.CursorName = cursorOrPath;
        }

        ApplyCursor();
    }

    /// <markdown-doc>
    /// <parent-name>Logic</parent-name>
    /// <path>Api/Logic</path>
    /// </markdown-doc>
    /// <summary>
    /// Restores all system cursors to the user's saved settings (undoes any previous `Cursor.Set` call).
    /// </summary>
    /// <markdown-example>
    /// <code language="lua">
    /// <![CDATA[
    /// Cursor.Restore()
    /// ]]>
    /// </code>
    /// </markdown-example>
    /// <name>Cursor.Restore</name>
    [ExposesScriptingMethod("Cursor.Restore")]
    public void RestoreForScript() => RestoreCursors();

    public override async Task Execute(AbstractInputBag inputBag = null) => ApplyCursor();

    public override string GetNameDisplay()
    {
        if (!string.IsNullOrEmpty(this.CursorFilePath))
        {
            return this.Name.Replace("{0}", Path.GetFileName(this.CursorFilePath));
        }

        if (!string.IsNullOrEmpty(this.CursorName))
        {
            return this.Name.Replace("{0}", this.CursorName);
        }

        return this.Name.Replace("{0}", "<hidden>");
    }

    public override bool Equals(object obj)
    {
        if (obj is not SetCursorAction other)
        {
            return false;
        }

        return other.CursorFilePath == this.CursorFilePath
            && other.CursorName == this.CursorName
            && other.Width == this.Width
            && other.Height == this.Height;
    }

    private void ApplyCursor()
    {
        // Start by restoring the default cursors, or changes won't apply correctly
        RestoreCursors();

        if (this.CursorName.Equals("RESET", StringComparison.OrdinalIgnoreCase))
        {
            // Special case: "Reset" is an alias for restoring the default cursors.
            return;
        }

        if (!string.IsNullOrEmpty(this.CursorFilePath))
        {
            SetCursorFromFile(this.CursorFilePath, this.Width, this.Height);
            return;
        }

        if (this.CursorName.Equals("HIDDEN", StringComparison.OrdinalIgnoreCase))
        {
            HideCursor();
            return;
        }

        var nameKey = this.CursorName.ToUpperInvariant().TrimStart("IDC_".ToCharArray());
        if (!string.IsNullOrEmpty(nameKey) && SystemCursorIds.TryGetValue(nameKey, out var namedId))
        {
            var shared = LoadCursor(IntPtr.Zero, (int)namedId);
            if (shared == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Could not load built-in cursor '{this.CursorName}'.");
            }

            foreach (var entry in SystemCursorIds)
            {
                var copy = CopyImage(shared, IMAGE_CURSOR, this.Width, this.Height, 0);
                SetSystemCursor(copy, entry.Value);
            }
            return;
        }

        HideCursor();
    }

    private static void HideCursor()
    {
        // A 32×32 cursor with AND=0xFF, XOR=0x00 is fully transparent.
        var andMask = new byte[128];
        var xorMask = new byte[128];

        for (var i = 0; i < andMask.Length; i++)
        {
            andMask[i] = 0xFF;
        }

        foreach (var entry in SystemCursorIds)
        {
            var handle = CreateCursor(IntPtr.Zero, 0, 0, 32, 32, andMask, xorMask);
            SetSystemCursor(handle, entry.Value);
        }
    }

    private static void SetCursorFromFile(string filePath, int width, int height)
    {
        var ext = Path.GetExtension(filePath).TrimStart('.').ToLowerInvariant();

        var uType = ext switch
        {
            "ani" or "cur" => IMAGE_CURSOR,
            "ico" => IMAGE_ICON,
            _ => throw new ArgumentException($"Unsupported cursor file type: '.{ext}'. Use .cur, .ani or .ico.")
        };

        if (ext == "ani")
        {
            // Animated cursors cannot be shared — load a fresh handle per slot.
            foreach (var entry in SystemCursorIds)
            {
                var handle = LoadImage(IntPtr.Zero, filePath, uType, width, height, LR_LOADFROMFILE);
                if (handle == IntPtr.Zero)
                {
                    throw new InvalidOperationException($"Failed to load animated cursor from '{filePath}'.");
                }

                SetSystemCursor(handle, entry.Value);
            }
        }
        else
        {
            // Static cursors: load once (shared), copy per slot.
            var shared = LoadImage(IntPtr.Zero, filePath, uType, width, height, LR_LOADFROMFILE_SHARED);
            if (shared == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Failed to load cursor from '{filePath}'. File may be corrupted.");
            }

            foreach (var entry in SystemCursorIds)
            {
                var copy = CopyImage(shared, IMAGE_CURSOR, 0, 0, 0);
                SetSystemCursor(copy, entry.Value);
            }
        }
    }

    private static void RestoreCursors() =>
        SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, 0);

    public override void OnStopListening(AbstractTriggerListener listener)
    {
        base.OnStopListening(listener);

        // Restore the cursor when any trigger stops listening, to avoid leaving the user stuck with a custom cursor if a script is interrupted.
        RestoreCursors();
    }
}
