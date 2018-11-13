using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using RabbitMQ.Client.Exceptions;
using Rebus.Bus;
using Rebus.Bus.Advanced;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Pipeline;

namespace Rebus
{
    public class ResilientBusDecorator : IBus
    {
        private readonly Policy retryPolicy;

        readonly IBus innerBus;
        private readonly ILog Logger;

        public ResilientBusDecorator(IBus bus, Policy policy=null,ILog logger=null)
        {
            this.innerBus = bus;
            Logger = logger;
            if (policy != null)
            {
                retryPolicy = policy;
            }
            else
            {
                retryPolicy= Policy.Handle<BrokerUnreachableException>()
                    .WaitAndRetryAsync(Enumerable.Repeat(TimeSpan.FromSeconds(1), 10),
                        (exception, delay, _) => Logger?.Info("Bus operation failed", exception));
            }
        }

        public async Task SendLocal(object commandMessage, Dictionary<string, string> optionalHeaders = null)
        {
            if (MessageContext.Current != null)
            {
                await innerBus.SendLocal(commandMessage, optionalHeaders);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.SendLocal(commandMessage, optionalHeaders));
        }

        public async Task Send(object commandMessage, Dictionary<string, string> additionalHeaders)
        {
            if (MessageContext.Current != null)
            {
                await innerBus.Send(commandMessage, additionalHeaders);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Send(commandMessage, additionalHeaders));
        }

        public async Task DeferLocal(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            if (MessageContext.Current != null)
            {
                await innerBus.DeferLocal(delay, message,optionalHeaders);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.DeferLocal(delay, message, optionalHeaders));
        }

        public async Task Defer(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            if (MessageContext.Current != null)
            {
                await innerBus.Defer(delay, message, optionalHeaders);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Defer(delay, message, optionalHeaders));
        }

        public async Task Reply(object replyMessage, Dictionary<string, string> optionalHeaders = null)
        {
            if (MessageContext.Current != null)
            {
                await innerBus.Reply(replyMessage, optionalHeaders);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Reply(replyMessage, optionalHeaders));
        }

        public async Task Subscribe<TEvent>()
        {
            if (MessageContext.Current != null)
            {
                await innerBus.Subscribe<TEvent>();
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Subscribe<TEvent>());
        }

        public async Task Subscribe(Type eventType)
        {
            if (MessageContext.Current != null)
            {
                await innerBus.Subscribe(eventType);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Subscribe(eventType));
        }

        public async Task Unsubscribe<TEvent>()
        {
            if (MessageContext.Current != null)
            {
                await innerBus.Unsubscribe<TEvent>();
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Unsubscribe<TEvent>());
        }

        public async Task Unsubscribe(Type eventType)
        {
            if (MessageContext.Current != null)
            {
                await innerBus.Unsubscribe(eventType);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Unsubscribe(eventType));
        }

        public async Task Publish(object eventMessage, Dictionary<string, string> optionalHeaders = null)
        {
            if (MessageContext.Current != null)
            {
                await innerBus.Publish(eventMessage, optionalHeaders);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Publish(eventMessage, optionalHeaders));
        }

        public IAdvancedApi Advanced => innerBus.Advanced;


        public void Dispose()
        {
            innerBus?.Dispose();
        }
    }
}
