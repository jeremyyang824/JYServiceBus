﻿using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Events.Bus;

namespace Wind.iSeller.Framework.Core.Configuration.Startup
{
    /// <summary>
    /// Used to configure <see cref="IEventBus"/>.
    /// </summary>
    public interface IEventBusConfiguration
    {
        /// <summary>
        /// True, to use <see cref="EventBus.Default"/>.
        /// False, to create per <see cref="IIocManager"/>.
        /// This is generally set to true. But, for unit tests,
        /// it can be set to false.
        /// Default: true.
        /// </summary>
        bool UseDefaultEventBus { get; set; }
    }
}