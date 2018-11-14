using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Rebus.Pipeline;
using Rebus.TestHelpers.Events;

namespace Rebus.RabbitMq.TransientFaultHelper.Test
{
    public class InMessageHandler_TestBusIsInvoked : BusTests
    {
        protected override void Setup()
        {
            decoratorBus = new TransientFaultBusDecorator(fakeInternalBus, policyMock.Object, ()=>new Mock<IMessageContext>().Object);
        }

        protected override void TestBeforeTearDown()
        {
            policyMock.Verify(x => x.ExecuteAsync(It.IsAny<Func<Task>>()), Times.Never);
        }
    }
}