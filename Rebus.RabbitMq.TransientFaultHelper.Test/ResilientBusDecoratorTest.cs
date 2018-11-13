using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Polly;
using Rebus.Bus;
using Rebus.Pipeline;
using Rebus.TestHelpers;
using Rebus.TestHelpers.Events;
using Rebus.Transport;

namespace Rebus.RabbitMq.TransientFaultHelper.Test
{
    [TestFixture]
    public abstract class ResilientBusDecoratorBaseTest
    {
        protected FakeBus fakeInternalBus;
        protected IBus decoratorBus;
        protected Moq.Mock<IAsyncPolicy> policyMock;
        protected Moq.Mock<Rebus.Logging.ILog> logMock;
        protected DummyMessage message = new DummyMessage() {Count = 1, Title = "Dummy message", Id = Guid.NewGuid(),};
        protected TimeSpan TimeSpan = TimeSpan.FromMinutes(1);

        protected abstract void Setup();
        protected abstract void TestBeforeTearDown();

        protected Dictionary<string, string> headers = new Dictionary<string, string>()
        {
            {"header1", "value1"}
        };

        [SetUp]
        public void BaseSetup()
        {
            logMock = new Mock<Rebus.Logging.ILog>();
            policyMock = new Mock<IAsyncPolicy>();
            fakeInternalBus = new FakeBus();

            logMock.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<object[]>()));


            Setup();
        }



        [TearDown]
        public void CleanUp()
        {
            TestBeforeTearDown();
            decoratorBus.Dispose();

        }

    }
}
