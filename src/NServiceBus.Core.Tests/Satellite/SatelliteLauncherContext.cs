﻿namespace NServiceBus.Core.Tests.Satellite
{
    using System.Reflection;
    using Fakes;
    using Faults;
    using NServiceBus.Config;
    using NUnit.Framework;
    using Satellites;
    using Unicast.Transport;

    public abstract class SatelliteLauncherContext
    {
        protected FuncBuilder Builder;
        protected IManageMessageFailures InMemoryFaultManager;
        protected FakeTransport Transport;
     
        [SetUp]
        public void SetUp()
        {
            Builder = new FuncBuilder();
            InMemoryFaultManager = new Faults.InMemory.FaultManager();
            Transport = new FakeTransport();

            Configure.With(new Assembly[0])
                .DefineEndpointName("Test")
                .DefaultBuilder();
            Configure.Instance.Builder = Builder;
           
            RegisterTypes();
            Builder.Register<IManageMessageFailures>(() => InMemoryFaultManager);
            Builder.Register<ITransport>(() => Transport);

            var configurer = new SatelliteConfigurer();
            configurer.Init();

            var launcher = new SatelliteLauncher
                               {
                                   Builder = Builder,
                               };

            BeforeRun();
            launcher.Start();
        }

        public abstract void BeforeRun();
        public abstract void RegisterTypes();
    }
}