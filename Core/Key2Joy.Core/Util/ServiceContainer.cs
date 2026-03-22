using System;
using System.Collections.Generic;

namespace Key2Joy.Util;

/// <summary>
/// A lightweight static service container that replaces the CommonServiceLocator dependency.
/// </summary>
public static class ServiceContainer
{
    private static readonly Dictionary<Type, object> services = new();

    public static void Register<T>(T service) => services[typeof(T)] = service;

    public static T Get<T>()
    {
        if (services.TryGetValue(typeof(T), out var service))
            return (T)service;

        throw new InvalidOperationException($"No registered service of type {typeof(T).FullName}");
    }
}
