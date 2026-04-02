# `ClearInterval` (```IntervalId```)

Cancels an interval previously established by calling SetInterval()

## Parameters
* **intervalId (```IntervalId```)** 
	Id returned by SetInterval to cancel


## Examples
> Shows how to count up to 3 every second and then stop by using ClearInterval();
> 
> #### _lua_:
> ```lua
> local count = 0
> local intervalId
> 
> intervalId = SetInterval(function ()
>    Print(count)
>    count = count + 1
> 
>    if (count == 3) then
>       ClearInterval(intervalId)
>    end
> end, 1000)
> 
> Print(intervalId);
> ```
---
