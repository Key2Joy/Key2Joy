# `SetTimeout` (```CallbackAction```, ```Int64```, ```Object[]```)

Timeout for the specified duration in milliseconds, then execute the callback

## Parameters
* **callback (```CallbackAction```)** 
	Function to execute after the wait

* **waitTime (```Int64```)** 
	Time to wait (in milliseconds)

* **arguments (```Object[]```)** 
	Zero or more extra parameters to pass to the function

## Returns
```TimeoutId```
A timeout id that can be removed with ClearTimeout.

## Examples
> Shows how to count down from 3 each second and execute a command using Lua.
> 
> #### _lua_:
> ```lua
> SetTimeout(function ()
>    Print("Aborting in 3 second...")
> 
>    SetTimeout(function ()
>       Print("Three")
> 
>       SetTimeout(function ()
>          Print("Two")
> 
>          SetTimeout(function ()
>             Print("One")
> 
>             SetTimeout(function ()
>                App.Command("abort")
>             end, 1000)
>          end, 1000)
>       end, 1000)
>    end, 1000)
> end, 1000)
> ```
---
