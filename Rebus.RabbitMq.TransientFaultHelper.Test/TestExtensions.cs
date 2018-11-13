using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rebus.TestHelpers;
using Rebus.TestHelpers.Events;

namespace Rebus.RabbitMq.TransientFaultHelper.Test
{
    internal static class TestExtensions
    {
        internal static void AssertEqual(this FakeBus bus, DummyMessage message,
            Dictionary<string, string> headers = null)
        {
            var events = bus.Events.OfType<MessageSent<DummyMessage>>();
            AssertEventList(events);
            var commandMessage = events.FirstOrDefault().CommandMessage;
            ValidateCommand(message, commandMessage, events.FirstOrDefault()?.OptionalHeaders, headers);
        }

        internal static void AssertEqualLocal(this FakeBus bus, DummyMessage message,
            Dictionary<string, string> headers = null)
        {
            var events = bus.Events.OfType<MessageSentToSelf<DummyMessage>>();
            AssertEventList(events);
            var commandMessage = events.FirstOrDefault().CommandMessage;
            ValidateCommand(message, commandMessage, events.FirstOrDefault()?.OptionalHeaders, headers);
        }


        internal static void AssertEqual(this FakeBus bus, DummyMessage message, TimeSpan timeSpan,
            Dictionary<string, string> headers = null)
        {
            var events = bus.Events.OfType<MessageDeferred<DummyMessage>>();
            AssertEventList(events);
            var commandMessage = events.FirstOrDefault().CommandMessage;
            ValidateCommand(message, commandMessage, events.FirstOrDefault()?.OptionalHeaders, headers);

            //Special check for defered messages
            Assert.AreEqual(events.FirstOrDefault().Delay, timeSpan);
        }


        internal static void AssertEqualLocal(this FakeBus bus, DummyMessage message, TimeSpan timeSpan,
            Dictionary<string, string> headers = null)
        {
            var events = bus.Events.OfType<MessageDeferredToSelf<DummyMessage>>();
            AssertEventList(events);
            var commandMessage = events.FirstOrDefault().CommandMessage;
            ValidateCommand(message, commandMessage, events.FirstOrDefault()?.OptionalHeaders, headers);

            //Special check for defered messages
            Assert.AreEqual(events.FirstOrDefault().Delay, timeSpan);
        }


        internal static void AssertReply(this FakeBus bus, DummyMessage message,
            Dictionary<string, string> headers = null)
        {
            var events = bus.Events.OfType<ReplyMessageSent<DummyMessage>>();
            AssertEventList(events);
            var commandMessage = events.FirstOrDefault().ReplyMessage;
            ValidateCommand(message, commandMessage, events.FirstOrDefault()?.OptionalHeaders, headers);
        }

        internal static void AssertPublish(this FakeBus bus, DummyMessage message,
            Dictionary<string, string> headers = null)
        {
            var events = bus.Events.OfType<MessagePublished<DummyMessage>>();
            AssertEventList(events);
            var commandMessage = events.FirstOrDefault().EventMessage;
            ValidateCommand(message, commandMessage, events.FirstOrDefault()?.OptionalHeaders, headers);
        }

        internal static void AssertSubscribe(this FakeBus bus, Type type)
        {
            var events = bus.Events.OfType<Subscribed>();
            AssertEventList(events);


            Assert.AreEqual(events.FirstOrDefault().EventType, type);
        }

        internal static void AssertUnsubscribe(this FakeBus bus, Type type)
        {
            var events = bus.Events.OfType<Unsubscribed>();
            AssertEventList(events);


            Assert.AreEqual(events.FirstOrDefault().EventType, type);
        }

        private static void AssertEventList<T>(IEnumerable<T> events)
        {
            Assert.IsNotNull(events);
            Assert.That(events.Count(), Is.EqualTo(1));
        }

        private static void ValidateCommand(DummyMessage message, DummyMessage commandMessage,
            Dictionary<string, string> fakeBusHeaders = null, Dictionary<string, string> headers = null)
        {
            message.AssertAreEqual(commandMessage);

            if (headers != null)
                foreach (var header in headers)
                {
                    Assert.IsTrue(fakeBusHeaders.ContainsKey(header.Key));
                    Assert.AreEqual(fakeBusHeaders[header.Key], header.Value);
                }
        }

        internal static void AssertAreEqual(this DummyMessage message, DummyMessage commandMessage)
        {
            Assert.AreEqual(commandMessage.Count, message.Count);
            Assert.AreEqual(commandMessage.Id, message.Id);
            Assert.AreEqual(commandMessage.Title, message.Title);            
        }
    }
}