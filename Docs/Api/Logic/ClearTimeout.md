# `ClearTimeout` (```TimeoutId```)

Cancels a timeout previously established by calling SetTimeout()

## Parameters
* **timeoutId (```TimeoutId```)** 
	Id returned by SetTimeout to cancel


## Examples
> Shows how to set and immediately cancel a timeout.
> 
> #### _lua_:
> ```lua
> local timeoutID = SetTimeout(function ()
>    Print("You shouldn't see this because the timeout will have been cancelled!")
> end, 1000);
> 
> Print(timeoutID)
> 
> ClearTimeout(timeoutID)
> ```
---
