using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.TestHelpers.Events;

namespace Rebus.RabbitMq.TransientFaultHelper.Test
{
    public class InMessageHandler_TestBusIsInvoked : BusTests
    {
        protected override void Setup()
        {
            decoratorBus = new ResilientBusDecorator(fakeInternalBus, policyMock.Object, logMock.Object);
        }

        protected override void TestBeforeTearDown()
        {
            //Nothing
        }
    }
}