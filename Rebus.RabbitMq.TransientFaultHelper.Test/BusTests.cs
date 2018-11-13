using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.TestHelpers;

namespace Rebus.RabbitMq.TransientFaultHelper.Test
{
    public abstract class BusTests: ResilientBusDecoratorBaseTest
    {
        
        [Test]
        public async Task Send()
        {
            await decoratorBus.Send(message);
            fakeInternalBus.AssertEqual(message);
        }


        [Test]
        public async Task SendWithHeaders()
        {
            await decoratorBus.Send(message, headers);
            fakeInternalBus.AssertEqual(message, headers);
        }

        [Test]
        public async Task SendLocal()
        {
            await decoratorBus.SendLocal(message);
            fakeInternalBus.AssertEqualLocal(message);
        }


        [Test]
        public async Task SendLocalWithHeaders()
        {
            await decoratorBus.SendLocal(message, headers);
            fakeInternalBus.AssertEqualLocal(message, headers);
        }



        [Test]
        public async Task Defer()
        {
            await decoratorBus.Defer(TimeSpan, message);
            fakeInternalBus.AssertEqual(message, TimeSpan);
        }


        [Test]
        public async Task DeferWithHeaders()
        {
            await decoratorBus.Defer(TimeSpan, message, headers);
            fakeInternalBus.AssertEqual(message, TimeSpan, headers);
        }

        [Test]
        public async Task DeferLocal()
        {
            await decoratorBus.DeferLocal(TimeSpan, message);
            fakeInternalBus.AssertEqualLocal(message, TimeSpan);
        }


        [Test]
        public async Task DeferLocalWithHeaders()
        {
            await decoratorBus.DeferLocal(TimeSpan, message, headers);
            fakeInternalBus.AssertEqualLocal(message, TimeSpan, headers);
        }



        [Test]
        public async Task Reply()
        {
            await decoratorBus.Reply(message);
            fakeInternalBus.AssertReply(message);
        }


        [Test]
        public async Task ReplyWithHeaders()
        {
            await decoratorBus.Reply(message, headers);
            fakeInternalBus.AssertReply(message, headers);
        }


        [Test]
        public async Task Subscripe()
        {
            await decoratorBus.Subscribe(typeof(DummyMessage));
            fakeInternalBus.AssertSubscribe(typeof(DummyMessage));
        }

        [Test]
        public async Task SubscripeGeneric()
        {
            await decoratorBus.Subscribe<DummyMessage>();
            fakeInternalBus.AssertSubscribe(typeof(DummyMessage));
        }

        [Test]
        public async Task Unsubscribe()
        {
            await decoratorBus.Unsubscribe(typeof(DummyMessage));
            fakeInternalBus.AssertUnsubscribe(typeof(DummyMessage));
        }

        [Test]
        public async Task UnsubscribeGeneric()
        {
            await decoratorBus.Unsubscribe<DummyMessage>();
            fakeInternalBus.AssertUnsubscribe(typeof(DummyMessage));
        }



        [Test]
        public async Task Publish()
        {
            await decoratorBus.Publish(message);
            fakeInternalBus.AssertPublish(message);
        }


        [Test]
        public async Task PublishWithHeaders()
        {
            await decoratorBus.Publish(message, headers);
            fakeInternalBus.AssertPublish(message, headers);
        }




    
    }
}