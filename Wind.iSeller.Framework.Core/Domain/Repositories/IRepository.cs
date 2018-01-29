using System;
using Wind.iSeller.Framework.Core.Dependency;

namespace Wind.iSeller.Framework.Core.Domain.Repositories
{
    /// <summary>
    /// This interface must be implemented by all repositories to identify them by convention.
    /// Implement generic version instead of this one.
    /// </summary>
    public interface IRepository : ITransientDependency
    {

    }
}
