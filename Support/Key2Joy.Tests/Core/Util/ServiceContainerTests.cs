using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    [TestInitialize]
    public void TestInitialize() => ServiceContainer.Reset();

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

    /// <summary>
    /// Proves that the static Dictionary is not isolated between tests.
    /// A service registered in one test leaks into the next: after
    /// Can_Override_Registered_Service runs, ISimulatedGamePadService is still
    /// mapped to TestAnotherGamePadService. This test expects a fresh
    /// SimulatedGamePadService but gets the stale one instead and fails.
    /// Fixed by adding a Reset API and calling it in [TestInitialize].
    /// </summary>
    [TestMethod]
    public void Isolation_Fails_Without_Reset_Between_Tests()
    {
        // We deliberately do NOT register anything here.
        // If the static dictionary were properly isolated, ISimulatedGamePadService
        // would be unregistered at this point and Get<T> would throw.
        bool threwExpected = false;
        try
        {
            ServiceContainer.Get<ISimulatedGamePadService>();
        }
        catch (InvalidOperationException)
        {
            threwExpected = true;
        }

        // This assertion documents the desired behaviour: without isolation the
        // container is still holding the stale entry from a previous test, so
        // threwExpected is false and the Assert fails.
        Assert.IsTrue(
            threwExpected,
            "Expected Get<ISimulatedGamePadService> to throw because no service was " +
            "registered in this test, but the stale registration from a previous test " +
            "was still present. The static Dictionary is not isolated between tests."
        );
    }

    /// <summary>
    /// Proves that concurrent Register and Get calls on the unsynchronized static
    /// Dictionary can corrupt its internal state, causing exceptions or returning
    /// wrong results. Fix: use ConcurrentDictionary or guard with a lock.
    /// </summary>
    [TestMethod]
    public void Concurrent_Register_And_Get_Does_Not_Corrupt_Dictionary()
    {
        const int iterations = 200_000;
        var exceptions = new System.Collections.Concurrent.ConcurrentBag<Exception>();

        // Writer thread: continuously re-registers two different services.
        var cts = new CancellationTokenSource();
        var writer = Task.Run(() =>
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    ServiceContainer.Register<ISimulatedGamePadService>(new SimulatedGamePadService());
                    ServiceContainer.Register<ISimulatedGamePadService>(new TestAnotherGamePadService());
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
        });

        // Reader threads: continuously call Get while the writer is mutating the dictionary.
        var readers = new Task[4];
        for (var i = 0; i < readers.Length; i++)
        {
            readers[i] = Task.Run(() =>
            {
                for (var j = 0; j < iterations; j++)
                {
                    try
                    {
                        ServiceContainer.Get<ISimulatedGamePadService>();
                    }
                    catch (InvalidOperationException)
                    {
                        // Acceptable: service temporarily unregistered between writes.
                    }
                    catch (Exception ex)
                    {
                        // NOT acceptable: internal Dictionary corruption (e.g. NullReferenceException,
                        // IndexOutOfRangeException, KeyNotFoundException from a torn read, etc.)
                        exceptions.Add(ex);
                    }
                }
            });
        }

        Task.WaitAll(readers);
        cts.Cancel();
        writer.Wait();

        Assert.IsEmpty(
            exceptions,
            $"Unexpected exceptions from concurrent access — Dictionary is not thread-safe. " +
            $"First exception: {(exceptions.IsEmpty ? "none" : exceptions.ToArray()[0].ToString())}"
        );
    }
}
