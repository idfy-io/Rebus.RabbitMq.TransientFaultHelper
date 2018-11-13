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
    public class TransientFaultBusDecorator : IBus
    {
        private readonly IAsyncPolicy retryPolicy;
        private readonly IBus innerBus;
        private readonly Action<string,Exception> LogException;
        private readonly ILog Logger =null;
        private readonly Func<IMessageContext> MessageContextWrapper;
        private readonly IAsyncPolicy defaultPolicy;

        /// <summary>
        /// Resilient Rebus decorator
        /// </summary>
        /// <param name="bus">The bus to decorate</param>
        /// <param name="policy">Retry policy, if not set default will be ued (10 times 1 second delay)</param>
        /// <param name="logger">Rebus logger</param>
        public TransientFaultBusDecorator(IBus bus, IAsyncPolicy policy=null,ILog logger=null)
        {
            this.innerBus = bus;
            Logger = logger;
            retryPolicy = policy ?? Policy.Handle<BrokerUnreachableException>()
                              .WaitAndRetryAsync(Enumerable.Repeat(TimeSpan.FromSeconds(1), 10),
                                  (exception, delay, _) => Logger?.Info("Bus operation failed", exception));
            
            MessageContextWrapper = () => MessageContext.Current;
        }

        /// <summary>
        /// Resilient Rebus decorator
        /// </summary>
        /// <param name="bus">The bus to decorate</param>
        /// <param name="policy">Retry policy, if not set default will be ued (10 times 1 second delay)</param>
        /// <param name="logException">Action to log exceptions</param>
        public TransientFaultBusDecorator(IBus bus, IAsyncPolicy policy = null, Action<string,Exception> logException=null)
        {
            this.innerBus = bus;
            this.LogException = logException;            
            retryPolicy = policy ?? Policy.Handle<BrokerUnreachableException>()
                              .WaitAndRetryAsync(Enumerable.Repeat(TimeSpan.FromSeconds(1), 10),
                                  (exception, delay, _) =>
                                  {
                                      logException?.Invoke("Bus operation failed", exception);
                                  });

            MessageContextWrapper = () => MessageContext.Current;
        }

        internal TransientFaultBusDecorator(IBus bus, IAsyncPolicy policy, ILog logger, Func<IMessageContext> messageContext)
        {
            this.innerBus = bus;
            Logger = logger;
            retryPolicy = policy ?? Policy.Handle<BrokerUnreachableException>()
                              .WaitAndRetryAsync(Enumerable.Repeat(TimeSpan.FromSeconds(1), 10),
                                  (exception, delay, _) => Logger?.Warn("Bus operation failed", exception));
            
            MessageContextWrapper = messageContext;
        }

        public async Task SendLocal(object commandMessage, Dictionary<string, string> optionalHeaders = null)
        {
            
            await Execute(() => innerBus.SendLocal(commandMessage, optionalHeaders));
        }

        public async Task Send(object commandMessage, Dictionary<string, string> additionalHeaders)
        {
            await Execute(() => innerBus.Send(commandMessage, additionalHeaders));
        }

        public async Task DeferLocal(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            await Execute(() => innerBus.DeferLocal(delay, message, optionalHeaders));
        }

        public async Task Defer(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null)
        {
            await Execute(() => innerBus.Defer(delay, message, optionalHeaders));
        }

        public async Task Reply(object replyMessage, Dictionary<string, string> optionalHeaders = null)
        {
            await Execute(() => innerBus.Reply(replyMessage, optionalHeaders));
        }

        public async Task Subscribe<TEvent>()
        {
            await Execute(() => innerBus.Subscribe<TEvent>());
        }

        public async Task Subscribe(Type eventType)
        {
            await Execute(() => innerBus.Subscribe(eventType));
        }

        public async Task Unsubscribe<TEvent>()
        {
            await Execute(() => innerBus.Unsubscribe<TEvent>());
        }

        public async Task Unsubscribe(Type eventType)
        {
            await Execute(() => innerBus.Unsubscribe(eventType));
        }

        public async Task Publish(object eventMessage, Dictionary<string, string> optionalHeaders = null)
        {         
            await Execute(() => innerBus.Publish(eventMessage, optionalHeaders));
        }

        private Task Execute(Func<Task> operation)
        {
            var currentMessageContext = MessageContextWrapper();

            Logger.Debug("Invoking {operation} in TransientFaultBusDecorator", operation.Method.Name);

            return currentMessageContext != null ? operation() : retryPolicy.ExecuteAsync(operation);
        }

        public IAdvancedApi Advanced => innerBus?.Advanced;


        public void Dispose()
        {
            innerBus?.Dispose();
        }
    }
}
