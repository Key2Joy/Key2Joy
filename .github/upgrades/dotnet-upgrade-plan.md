# .NET 9.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 9.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 9.0 upgrade.
3. Add the local NuGet feed (`C:\LocalNuGet`) to NuGet.Config as `LocalFeed` package source.
4. Upgrade Core\Key2Joy.Contracts\Key2Joy.Contracts.csproj
5. Upgrade Core\Key2Joy.Core\Key2Joy.Core.csproj
6. Upgrade Key2Joy.Cmd\Key2Joy.Cmd.csproj
7. Upgrade Key2Joy.Gui\Key2Joy.Gui.csproj
8. Upgrade Support\BuildMarkdownDocs\BuildMarkdownDocs.csproj
9. Upgrade Support\Key2Joy.Tests\Key2Joy.Tests.csproj
10. Upgrade Support\Key2Joy.Setup\Key2Joy.Setup.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

None.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                        | Current Version | New Version | Description                                                      |
|:------------------------------------|:---------------:|:-----------:|:-----------------------------------------------------------------|
| Microsoft.NETCore.Platforms         | 7.0.1           |             | Remove — functionality included with new framework reference     |
| NETStandard.Library                 | 2.0.3           |             | Remove — functionality included with new framework reference     |
| ObjectListView.Official             | 2.9.1           |             | Replace with ObjectListView.Repack.Core3                         |
| ObjectListView.Repack.Core3         |                 | 2.9.3       | Replacement for ObjectListView.Official                          |
| SimWinGamePad                       | 1.1.1           | 1.0.0       | Update to 1.0.0 from local NuGet feed (C:\LocalNuGet)            |
| System.Collections                  | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Collections.Concurrent       | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Diagnostics.Debug            | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Diagnostics.Tools            | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Globalization                | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.IO.Compression               | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.IO.Compression.ZipFile       | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Management                   | 10.0.0          | 9.0.14      | Downgrade to 9.0.14 — recommended for .NET 9.0                   |
| System.Net.Primitives               | 4.3.1           |             | Remove — functionality included with new framework reference     |
| System.ObjectModel                  | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Reflection.Extensions        | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Reflection.Primitives        | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Resources.Extensions         | 7.0.0           | 9.0.14      | Update to 9.0.14 — recommended for .NET 9.0                      |
| System.Resources.ResourceManager    | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Runtime.Numerics             | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Text.Encoding                | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Text.Encoding.Extensions     | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Text.Json                    | 10.0.5          | 9.0.14      | Downgrade to 9.0.14 — recommended for .NET 9.0                   |
| System.Threading                    | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Threading.Tasks              | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Threading.Timer              | 4.3.0           |             | Remove — functionality included with new framework reference     |
| System.Xml.XDocument                | 4.3.0           |             | Remove — functionality included with new framework reference     |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### Core\Key2Joy.Contracts\Key2Joy.Contracts.csproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net9.0-windows`

NuGet packages changes:
  - `System.Text.Json` should be updated from `10.0.5` to `9.0.14` (recommended for .NET 9.0)

#### Core\Key2Joy.Core\Key2Joy.Core.csproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net9.0-windows`

NuGet packages changes:
  - `SimWinGamePad` should be updated from `1.1.1` to `1.0.0` from local NuGet feed (`C:\LocalNuGet`)
  - Remove `Microsoft.NETCore.Platforms` 7.0.1 (functionality included with new framework reference)
  - Remove `NETStandard.Library` 2.0.3 (functionality included with new framework reference)
  - Remove `System.Collections` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Collections.Concurrent` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Diagnostics.Debug` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Diagnostics.Tools` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Globalization` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Net.Primitives` 4.3.1 (functionality included with new framework reference)
  - Remove `System.ObjectModel` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Reflection.Extensions` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Reflection.Primitives` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Resources.ResourceManager` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Runtime.Numerics` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Text.Encoding` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Text.Encoding.Extensions` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Threading` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Threading.Tasks` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Threading.Timer` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Xml.XDocument` 4.3.0 (functionality included with new framework reference)

#### Key2Joy.Cmd\Key2Joy.Cmd.csproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net9.0-windows`

NuGet packages changes:
  - `System.Resources.Extensions` should be updated from `7.0.0` to `9.0.14` (recommended for .NET 9.0)

#### Key2Joy.Gui\Key2Joy.Gui.csproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net9.0-windows`

NuGet packages changes:
  - `ObjectListView.Official` 2.9.1 must be replaced with `ObjectListView.Repack.Core3` 2.9.3
  - `System.Management` should be updated from `10.0.0` to `9.0.14` (recommended for .NET 9.0)
  - Remove `Microsoft.NETCore.Platforms` 7.0.1 (functionality included with new framework reference)
  - Remove `NETStandard.Library` 2.0.3 (functionality included with new framework reference)
  - Remove `System.Collections` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Collections.Concurrent` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Diagnostics.Debug` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Diagnostics.Tools` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Globalization` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Net.Primitives` 4.3.1 (functionality included with new framework reference)
  - Remove `System.ObjectModel` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Reflection.Extensions` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Reflection.Primitives` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Resources.ResourceManager` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Runtime.Numerics` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Text.Encoding` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Text.Encoding.Extensions` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Threading` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Threading.Tasks` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Threading.Timer` 4.3.0 (functionality included with new framework reference)
  - Remove `System.Xml.XDocument` 4.3.0 (functionality included with new framework reference)

#### Support\BuildMarkdownDocs\BuildMarkdownDocs.csproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net9.0`

#### Support\Key2Joy.Tests\Key2Joy.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net9.0-windows`

#### Support\Key2Joy.Setup\Key2Joy.Setup.csproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net9.0-windows`

NuGet packages changes:
  - `System.Text.Json` should be updated from `10.0.5` to `9.0.14` (recommended for .NET 9.0)
  - Remove `System.IO.Compression` 4.3.0 (functionality included with new framework reference)
  - Remove `System.IO.Compression.ZipFile` 4.3.0 (functionality included with new framework reference)
