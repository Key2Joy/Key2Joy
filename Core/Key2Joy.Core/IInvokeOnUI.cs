using System;

namespace Key2Joy;

public interface IInvokeOnUI
{
    object Invoke(Delegate method);

    object Invoke(Delegate method, params object[] arguments);
}
