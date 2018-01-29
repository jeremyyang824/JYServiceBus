﻿using System;
using Wind.iSeller.Framework.Core.Events.Bus.Factories;
using Wind.iSeller.Framework.Core.Events.Bus.Handlers;
using Wind.iSeller.Framework.Core.Utils;

namespace Wind.iSeller.Framework.Core.Events.Bus
{
    /// <summary>
    /// An event bus that implements Null object pattern.
    /// </summary>
    public sealed class NullEventBus : IEventBus
    {
        /// <summary>
        /// Gets single instance of <see cref="NullEventBus"/> class.
        /// </summary>
        public static NullEventBus Instance { get { return SingletonInstance; } }
        private static readonly NullEventBus SingletonInstance = new NullEventBus();

        private NullEventBus()
        {
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData, THandler>()
            where TEventData : IEventData
            where THandler : IEventHandler<TEventData>, new()
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register(Type eventType, IEventHandler handler)
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(IEventHandlerFactory handlerFactory) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register(Type eventType, IEventHandlerFactory handlerFactory)
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandler handler)
        {
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandlerFactory factory)
        {
        }

        /// <inheritdoc/>
        public void UnregisterAll<TEventData>() where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void UnregisterAll(Type eventType)
        {
        }

        /// <inheritdoc/>
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Trigger(Type eventType, IEventData eventData)
        {
        }

        /// <inheritdoc/>
        public void Trigger(Type eventType, object eventSource, IEventData eventData)
        {
        }
    }
}
