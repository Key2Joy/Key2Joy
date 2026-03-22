using System;
using System.Collections.Generic;
using Key2Joy.LowLevelInput;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Key2Joy.Tests.Core.Util;

public interface INonExistingService
{ }

public interface IAnotherService
{ }

public class TestService : IAnotherService
{ }

public class TestAnotherGamePadService : ISimulatedGamePadService
{
    public void Initialize()
    { }

    public void ShutDown()
    { }

    public ISimulatedGamePad GetGamePad(int gamePadIndex) => null;

    public ISimulatedGamePad[] GetAllGamePads(bool onlyPluggedIn = true) => Array.Empty<ISimulatedGamePad>();

    public void EnsureAllUnplugged()
    { }

    public void EnsurePluggedIn(int gamePadIndex)
    { }

    public void EnsureUnplugged(int gamePadIndex)
    { }

    public IList<IGamePadInfo> GetActiveDevicesInfo() => Array.Empty<IGamePadInfo>();
}

[TestClass]
public class ServiceContainerTests
{
    [TestMethod]
    public void Register_And_Retrieve_Service()
    {
        var instance = new SimulatedGamePadService();
        ServiceContainer.Register<ISimulatedGamePadService>(instance);

        var service = ServiceContainer.Get<ISimulatedGamePadService>();
        Assert.IsNotNull(service);
        Assert.IsInstanceOfType(service, typeof(SimulatedGamePadService));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Retrieve_Unregistered_Service_Throws_Exception()
    {
        // Use an interface that is certainly not registered in this test
        ServiceContainer.Get<INonExistingService>();
    }

    [TestMethod]
    public void Can_Register_Multiple_Services()
    {
        ServiceContainer.Register<ISimulatedGamePadService>(new SimulatedGamePadService());
        ServiceContainer.Register<IAnotherService>(new TestService());

        Assert.IsNotNull(ServiceContainer.Get<ISimulatedGamePadService>());
        Assert.IsNotNull(ServiceContainer.Get<IAnotherService>());
    }

    [TestMethod]
    public void Registered_Service_Is_Singleton_By_Default()
    {
        ServiceContainer.Register<ISimulatedGamePadService>(new SimulatedGamePadService());

        var service1 = ServiceContainer.Get<ISimulatedGamePadService>();
        var service2 = ServiceContainer.Get<ISimulatedGamePadService>();

        Assert.AreSame(service1, service2);
    }

    [TestMethod]
    public void Can_Override_Registered_Service()
    {
        ServiceContainer.Register<ISimulatedGamePadService>(new SimulatedGamePadService());
        ServiceContainer.Register<ISimulatedGamePadService>(new TestAnotherGamePadService());

        var service = ServiceContainer.Get<ISimulatedGamePadService>();

        Assert.IsInstanceOfType(service, typeof(TestAnotherGamePadService));
    }
}
