using System;
using System.Collections.Generic;

namespace Wind.iSeller.Framework.Core.Application.Services
{
    public abstract class ApplicationService : WindServiceBase, IApplicationService, IAvoidDuplicateCrossCuttingConcerns
    {
        public static string[] CommonPostfixes = { "AppService", "ApplicationService" };

        /// <summary>
        /// Gets current session information.
        /// </summary>
        //public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// Gets the applied cross cutting concerns.
        /// </summary>
        public List<string> AppliedCrossCuttingConcerns { get; private set; }

        protected ApplicationService()
        {
            this.AppliedCrossCuttingConcerns = new List<string>();
        }
    }
}
