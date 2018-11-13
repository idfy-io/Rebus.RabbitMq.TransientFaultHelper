using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Polly;
using RabbitMQ.Client.Exceptions;
using Rebus.Bus;
using Rebus.Bus.Advanced;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Pipeline;

[assembly: InternalsVisibleTo("Rebus.RabbitMq.TransientFaultHelper.Test")]
namespace Rebus
{
    /// <summary>
    /// RabbitMQ resilient Rebus decorator
    /// </summary>
    public class ResilientBusDecorator : IBus
    {
        private readonly IAsyncPolicy retryPolicy;

        readonly IBus innerBus;
        private readonly ILog Logger;
        private readonly Func<IMessageContext> MessageContextWrapper;
        
        /// <summary>
        /// Resilient Rebus decorator
        /// </summary>
        /// <param name="bus">The bus to decorate</param>
        /// <param name="policy">Polly policy if not set will use default 1 seconds 10 times</param>
        /// <param name="logger">Rebus logger</param>
        public ResilientBusDecorator(IBus bus, IAsyncPolicy policy=null,ILog logger=null)
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


            MessageContextWrapper = () => MessageContext.Current;
        }

        internal ResilientBusDecorator(IBus bus, IAsyncPolicy policy, ILog logger, Func<IMessageContext> messageContext)
        {
            this.innerBus = bus;
            Logger = logger;
            
            if (policy != null)
            {
                retryPolicy = policy;
            }
            else
            {
                retryPolicy = Policy.Handle<BrokerUnreachableException>()
                    .WaitAndRetryAsync(Enumerable.Repeat(TimeSpan.FromSeconds(1), 10),
                        (exception, delay, _) => Logger?.Info("Bus operation failed", exception));
            }


            MessageContextWrapper = messageContext;
        }

        public async Task SendLocal(object commandMessage, Dictionary<string, string> optionalHeaders = null)
        {
            if (MessageContextWrapper != null)
            {
                await innerBus.SendLocal(commandMessage, optionalHeaders);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.SendLocal(commandMessage, optionalHeaders));
        }

        public async Task Send(object commandMessage, Dictionary<string, string> additionalHeaders)
        {
            if (MessageContextWrapper != null)
            {
                await innerBus.Send(commandMessage, additionalHeaders);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Send(commandMessage, additionalHeaders));
        }

        public async Task DeferLocal(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            if (MessageContextWrapper != null)
            {
                await innerBus.DeferLocal(delay, message,optionalHeaders);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.DeferLocal(delay, message, optionalHeaders));
        }

        public async Task Defer(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            if (MessageContextWrapper != null)
            {
                await innerBus.Defer(delay, message, optionalHeaders);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Defer(delay, message, optionalHeaders));
        }

        public async Task Reply(object replyMessage, Dictionary<string, string> optionalHeaders = null)
        {
            if (MessageContextWrapper != null)
            {
                await innerBus.Reply(replyMessage, optionalHeaders);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Reply(replyMessage, optionalHeaders));
        }

        public async Task Subscribe<TEvent>()
        {
            if (MessageContextWrapper != null)
            {
                await innerBus.Subscribe<TEvent>();
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Subscribe<TEvent>());
        }

        public async Task Subscribe(Type eventType)
        {
            if (MessageContextWrapper != null)
            {
                await innerBus.Subscribe(eventType);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Subscribe(eventType));
        }

        public async Task Unsubscribe<TEvent>()
        {
            if (MessageContextWrapper != null)
            {
                await innerBus.Unsubscribe<TEvent>();
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Unsubscribe<TEvent>());
        }

        public async Task Unsubscribe(Type eventType)
        {
            if (MessageContextWrapper != null)
            {
                await innerBus.Unsubscribe(eventType);
                return;
            }

            await retryPolicy.ExecuteAsync(() => innerBus.Unsubscribe(eventType));
        }

        public async Task Publish(object eventMessage, Dictionary<string, string> optionalHeaders = null)
        {
            if (MessageContextWrapper != null)
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
