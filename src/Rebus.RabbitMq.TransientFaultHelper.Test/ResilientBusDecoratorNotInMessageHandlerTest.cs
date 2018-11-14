using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Rebus.Bus;
using Rebus.TestHelpers;

namespace Rebus.RabbitMq.TransientFaultHelper.Test
{
    public class ResilientBusDecoratorNotInMessageHandlerTest : ResilientBusDecoratorBaseTest
    {
        protected override void Setup()
        {
            decoratorBus = new TransientFaultBusDecorator(fakeInternalBus, policyMock.Object, Log);
        }

        private void Log(string message, Exception exception)
        {
            Console.WriteLine(message);
            Console.WriteLine(exception);
        }


        [Test]
        public async Task Send()
        {
            await decoratorBus.Send(message);
        }


        [Test]
        public async Task SendWithHeaders()
        {
            await decoratorBus.Send(message, headers);
        }

        [Test]
        public async Task SendLocal()
        {
            await decoratorBus.SendLocal(message);
        }


        [Test]
        public async Task SendLocalWithHeaders()
        {
            await decoratorBus.SendLocal(message, headers);
        }


        [Test]
        public async Task Defer()
        {
            await decoratorBus.Defer(TimeSpan, message);
        }


        [Test]
        public async Task DeferWithHeaders()
        {
            await decoratorBus.Defer(TimeSpan, message, headers);
        }

        [Test]
        public async Task DeferLocal()
        {
            await decoratorBus.DeferLocal(TimeSpan, message);
        }


        [Test]
        public async Task DeferLocalWithHeaders()
        {
            await decoratorBus.DeferLocal(TimeSpan, message, headers);
        }


        [Test]
        public async Task Reply()
        {
            await decoratorBus.Reply(message);
        }


        [Test]
        public async Task ReplyWithHeaders()
        {
            await decoratorBus.Reply(message, headers);
        }


        [Test]
        public async Task Subscripe()
        {
            await decoratorBus.Subscribe(typeof(DummyMessage));
        }

        [Test]
        public async Task SubscripeGeneric()
        {
            await decoratorBus.Subscribe<DummyMessage>();
        }

        [Test]
        public async Task Unsubscribe()
        {
            await decoratorBus.Unsubscribe(typeof(DummyMessage));
        }

        [Test]
        public async Task UnsubscribeGeneric()
        {
            await decoratorBus.Unsubscribe<DummyMessage>();
        }


        [Test]
        public async Task Publish()
        {
            await decoratorBus.Publish(message);
        }


        [Test]
        public async Task PublishWithHeaders()
        {
            await decoratorBus.Publish(message, headers);
        }

        protected override void TestBeforeTearDown()
        {
            policyMock.Verify(x=>x.ExecuteAsync(It.IsAny<Func<Task>>()),Times.Once);
        }
    }
}