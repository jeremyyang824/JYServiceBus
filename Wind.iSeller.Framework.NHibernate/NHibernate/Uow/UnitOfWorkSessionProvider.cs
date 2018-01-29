using NHibernate;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Domain.Uow;

namespace Wind.iSeller.Framework.NHibernate.Uow
{
    public class UnitOfWorkSessionProvider : ISessionProvider, ITransientDependency
    {
        public ISession Session
        {
            get { return _unitOfWorkProvider.Current.GetSession(); }
        }
        
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;

        public UnitOfWorkSessionProvider(ICurrentUnitOfWorkProvider unitOfWorkProvider)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
        }
    }
}