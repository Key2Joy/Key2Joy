# `Cursor.Set` (```String```, ```Int32```, ```Int32```)

Changes every system cursor.
- Pass no arguments (or an empty string) to **hide** the cursor entirely.
- Pass a built-in cursor name such as `"Arrow"`, `"Hand"` or `"Wait"` to replace all cursors with that shape.
- Pass a file path to a `.cur`, `.ani`, or `.ico` file to use a custom cursor image.
Call `Cursor.Restore` to undo any changes made by this method.

## Parameters
* **cursorOrPath (```String```)** 
	
             Built-in cursor name, path to a .cur/.ani/.ico file, or empty to hide the cursor.
             

* **width (```Int32```)** 
	Width in pixels (0 = system default).

* **height (```Int32```)** 
	Height in pixels (0 = system default).


## Examples
> Replaces every cursor with the built-in Wait (hourglass) cursor.
> 
> #### _lua_:
> ```lua
> Cursor.Set("Wait")
> ```
---
> Loads a custom cursor from disk.
> 
> #### _lua_:
> ```lua
> Cursor.Set("C:\\cursors\\my_cursor.ani")
> ```
---
