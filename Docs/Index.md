# Scripting API Reference

## Enumerations

* [`AppCommand`](Api/Enumerations/AppCommand.md)
* [`Buttons`](Api/Enumerations/Buttons.md)
* [`GamePadControl`](Api/Enumerations/GamePadControl.md)
* [`GamePadSide`](Api/Enumerations/GamePadSide.md)
* [`KeyboardKey`](Api/Enumerations/KeyboardKey.md)
* [`MoveType`](Api/Enumerations/MoveType.md)
* [`PressState`](Api/Enumerations/PressState.md)

## Graphics

* [`Graphics.CaptureScreen` (```String```, ```Int32?```, ```Int32?```, ```Int32?```, ```Int32?```)](Api/Graphics/Graphics.CaptureScreen.md)
* [`Graphics.GetPixelColor` (```Int32```, ```Int32```)](Api/Graphics/Graphics.GetPixelColor.md)

## Input

* [`Cursor.GetPosition` ()](Api/Input/Cursor.GetPosition.md)
* [`GamePad.Reset` (```Int32```)](Api/Input/GamePad.Reset.md)
* [`GamePad.Simulate` (```GamePadControl```, ```PressState```, ```Int32```)](Api/Input/GamePad.Simulate.md)
* [`GamePad.SimulateMove` (```Int16```, ```Int16```, ```GamePadSide```, ```Int32```)](Api/Input/GamePad.SimulateMove.md)
* [`GamePad.SimulateTrigger` (```Single```, ```GamePadSide```, ```Int32```)](Api/Input/GamePad.SimulateTrigger.md)
* [`Keyboard.GetKeyDown` (```KeyboardKey```)](Api/Input/Keyboard.GetKeyDown.md)
* [`Keyboard.Simulate` (```KeyboardKey```, ```PressState```)](Api/Input/Keyboard.Simulate.md)
* [`Mouse.GetButtonDown` (```Buttons```)](Api/Input/Mouse.GetButtonDown.md)
* [`Mouse.Simulate` (```Buttons```, ```PressState```)](Api/Input/Mouse.Simulate.md)
* [`Mouse.SimulateMove` (```Int32```, ```Int32```, ```MoveType```)](Api/Input/Mouse.SimulateMove.md)

## Logic

* [`App.Command` (```AppCommand```)](Api/Logic/App.Command.md)
* [`ClearInterval` (```IntervalId```)](Api/Logic/ClearInterval.md)
* [`ClearTimeout` (```TimeoutId```)](Api/Logic/ClearTimeout.md)
* [`Cursor.Restore` ()](Api/Logic/Cursor.Restore.md)
* [`Cursor.Set` (```String```, ```Int32```, ```Int32```)](Api/Logic/Cursor.Set.md)
* [`MessageBox.Show` (```String```)](Api/Logic/MessageBox.Show.md)
* [`SetDelayedFunctions` (```Int64```, ```Action[]```)](Api/Logic/SetDelayedFunctions.md)
* [`SetInterval` (```CallbackAction```, ```Int64```, ```Object[]```)](Api/Logic/SetInterval.md)
* [`SetTimeout` (```CallbackAction```, ```Int64```, ```Object[]```)](Api/Logic/SetTimeout.md)

## Util

* [`Util.GetUnixTimeSeconds` ()](Api/Util/Util.GetUnixTimeSeconds.md)
* [`Util.PathExpand` (```String```)](Api/Util/Util.PathExpand.md)

## Windows

* [`Window.Find` (```String```, ```String```)](Api/Windows/Window.Find.md)
* [`Window.Focus` (```String```, ```String```)](Api/Windows/Window.Focus.md)
* [`Window.GetAll` ()](Api/Windows/Window.GetAll.md)
* [`Window.GetClass` (```IntPtr```)](Api/Windows/Window.GetClass.md)
* [`Window.GetForeground` ()](Api/Windows/Window.GetForeground.md)
* [`Window.GetTitle` (```IntPtr```)](Api/Windows/Window.GetTitle.md)
* [`Window.Minimize` (```String```, ```String```)](Api/Windows/Window.Minimize.md)

