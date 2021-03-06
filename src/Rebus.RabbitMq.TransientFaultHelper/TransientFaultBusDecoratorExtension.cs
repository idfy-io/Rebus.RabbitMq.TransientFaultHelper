﻿using System;
using Polly;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;

namespace Rebus
{
    public static class TransientFaultBusDecoratorConfigurationExtensions
    {
        /// <summary>
        /// Add the transient fault decorator to the Rebus pipeline 
        /// </summary>
        /// <param name="options">Rebus options</param>
        /// <param name="policy"></param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static OptionsConfigurer AddTransientFaultBus(this OptionsConfigurer options, IAsyncPolicy policy = null, IRebusLoggerFactory loggerFactory=null)
        {
            options.Decorate<IBus>(c => new TransientFaultBusDecorator(c.Get<IBus>(), policy, loggerFactory ?? c.Get<IRebusLoggerFactory>()));
            return options;
        }


        /// <summary>
        /// Add the transient fault decorator to the Rebus pipeline 
        /// </summary>
        /// <param name="options">Rebus options</param>        
        /// <param name="logException">Action to log exceptions</param>
        /// <returns></returns>
        public static OptionsConfigurer AddTransientFaultBus(this OptionsConfigurer options,Action<string, Exception> logException = null)
        {
            options.Decorate<IBus>(c => new TransientFaultBusDecorator(c.Get<IBus>(),  logException));
            return options;
        }
    }
}