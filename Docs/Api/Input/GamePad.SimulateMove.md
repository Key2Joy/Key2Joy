# `GamePad.SimulateMove` (```Single```, ```Single```, ```GamePadSide```, ```Int32```)

Simulate moving a gamepad joystick

## Parameters
* **deltaX (```Single```)** 
	The fraction (-1 to 1) by which to move the stick right (positive) or left (negative)

* **deltaY (```Single```)** 
	The fraction (-1 to 1) by which to move the stick up (positive) or down (negative)

* **side (```GamePadSide```)** 
	Which gamepad stick to move, either GamePadSide.Left (default) or .Right

* **gamepadIndex (```Int32```)** 
	Which of 4 possible gamepads to simulate: 0 (default), 1, 2 or 3


## Examples
> Moves the left gamepad joystick halfway up and to the right, then resets after 500ms
> 
> #### _lua_:
> ```lua
> GamePad.SimulateMove(0.5,0.5)
> SetTimeout(function()
>    GamePad.Reset()
> end, 500)
> ```
---
