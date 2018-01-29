using NHibernate;

namespace Wind.iSeller.Framework.NHibernate
{
    public interface ISessionProvider
    {
        ISession Session { get; }
    }
}