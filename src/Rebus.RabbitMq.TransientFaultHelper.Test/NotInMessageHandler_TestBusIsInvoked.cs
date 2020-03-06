using System;
using Polly;
using Polly.NoOp;

namespace Rebus.RabbitMq.TransientFaultHelper.Test
{
    public class NotInMessageHandler_TestBusIsInvoked : BusTests
    {
        AsyncNoOpPolicy noOp = Policy.NoOpAsync();
        protected override void Setup()
        {
            
            decoratorBus = new TransientFaultBusDecorator(fakeInternalBus, noOp,Log);
        }

        protected override void TestBeforeTearDown()
        {
            
        }

        private void Log(string message, Exception exception)
        {
            Console.WriteLine(message);
            Console.WriteLine(exception);
        }
    }


}