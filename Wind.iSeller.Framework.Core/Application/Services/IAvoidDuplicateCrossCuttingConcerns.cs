using System;
using System.Collections.Generic;

namespace Wind.iSeller.Framework.Core.Application.Services
{
    public interface IAvoidDuplicateCrossCuttingConcerns
    {
        List<string> AppliedCrossCuttingConcerns { get; }
    }
}
