using Polly;
using Polly.NoOp;

namespace Rebus.RabbitMq.TransientFaultHelper.Test
{
    public class NotInMessageHandler_TestBusIsInvoked : BusTests
    {
        NoOpPolicy noOp = Policy.NoOpAsync();
        protected override void Setup()
        {
            
            decoratorBus = new ResilientBusDecorator(fakeInternalBus, noOp, logMock.Object, null);
        }

        protected override void TestBeforeTearDown()
        {
            
        }
    }


}