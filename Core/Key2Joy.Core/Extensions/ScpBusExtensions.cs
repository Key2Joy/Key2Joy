using System.IO;
using Esprima.Ast;
using SimWinInput;

namespace Key2Joy.Extensions;

/// <summary>
/// Extensions for ScpBus.
/// </summary>
public static class ScpBusExtensions
{
    extension(ScpBus scpBus)
    {
        /// <summary>
        /// Checks if the necessary driver for simulated gamepads is installed.
        /// </summary>
        /// <returns>
        /// True if the driver is installed; otherwise, false.
        /// </returns>
        public static bool IsDriverInstalled()
        {
            try
            {
                var bus = new ScpBus();
                bus.Dispose();
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
