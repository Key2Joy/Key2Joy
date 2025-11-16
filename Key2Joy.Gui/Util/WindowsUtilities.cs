using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace Key2Joy.Gui.Util;

/// <summary>
/// Provides utility methods for Windows-specific operations.
/// </summary>
internal static class WindowsUtilities
{
    /// <summary>
    ///     The type of event. For more information, see <see cref="CreateRestorePoint"/>.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        ///     A system change has begun. A subsequent nested call does not create a new restore
        ///     point.
        ///     <para>
        ///         Subsequent calls must use <see cref="EventType.EndNestedSystemChange"/>, not
        ///         <see cref="EventType.EndSystemChange"/>.
        ///     </para>
        /// </summary>
        BeginNestedSystemChange = 0x66,

        /// <summary>
        ///     A system change has begun.
        /// </summary>
        BeginSystemChange = 0x64,

        /// <summary>
        ///     A system change has ended.
        /// </summary>
        EndNestedSystemChange = 0x67,

        /// <summary>
        ///     A system change has ended.
        /// </summary>
        EndSystemChange = 0x65
    }

    /// <summary>
    ///     The type of restore point. For more information, see <see cref="CreateRestorePoint"/>.
    /// </summary>
    public enum RestorePointType
    {
        /// <summary>
        ///     An application has been installed.
        /// </summary>
        ApplicationInstall = 0x0,

        /// <summary>
        ///     An application has been uninstalled.
        /// </summary>
        ApplicationUninstall = 0x1,

        /// <summary>
        ///     An application needs to delete the restore point it created. For example, an
        ///     application would use this flag when a user cancels an installation. 
        /// </summary>
        CancelledOperation = 0xd,

        /// <summary>
        ///     A device driver has been installed.
        /// </summary>
        DeviceDriverInstall = 0xa,

        /// <summary>
        ///     An application has had features added or removed.
        /// </summary>
        ModifySettings = 0xc
    }

    /// <summary>
    ///     Creates a restore point on the local system.
    /// </summary>
    /// <param name="description">
    ///     The description to be displayed so the user can easily identify a restore point.
    /// </param>
    /// <param name="eventType">
    ///     The type of event.
    /// </param>
    /// <param name="restorePointType">
    ///     The type of restore point. 
    /// </param>
    /// <exception cref="ManagementException">
    ///     Access denied.
    /// </exception>
    /// https://stackoverflow.com/a/42733327
    public static void CreateRestorePoint(string description, EventType eventType, RestorePointType restorePointType)
    {
        var mScope = new ManagementScope("\\\\localhost\\root\\default");
        var mPath = new ManagementPath("SystemRestore");
        var options = new ObjectGetOptions();
        using (var mClass = new ManagementClass(mScope, mPath, options))
        using (var parameters = mClass.GetMethodParameters("CreateRestorePoint"))
        {
            parameters["Description"] = description;
            parameters["EventType"] = (int)eventType;
            parameters["RestorePointType"] = (int)restorePointType;
            mClass.InvokeMethod("CreateRestorePoint", parameters, null);
        }
    }

}
