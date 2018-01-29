using System;
using Wind.iSeller.Framework.Core;

namespace Wind.iSeller.NServiceBus.ZeroService.Domain
{
    public class ScriptProxyGeneratorException : WindException
    {
        public ScriptProxyGeneratorException(string message)
            : base(message)
        { }
    }
}
