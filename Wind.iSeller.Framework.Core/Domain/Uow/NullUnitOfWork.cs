
namespace Wind.iSeller.Framework.Core.Domain.Uow
{
    /// <summary>
    /// Null implementation of unit of work.
    /// It's used if no component registered for <see cref="IUnitOfWork"/>.
    /// This ensures working Wind without a database.
    /// </summary>
    public sealed class NullUnitOfWork : UnitOfWorkBase
    {
        public override void SaveChanges()
        {

        }

        protected override void BeginUow()
        {

        }

        protected override void CompleteUow()
        {

        }

        protected override void DisposeUow()
        {

        }

        public NullUnitOfWork(
            IConnectionStringResolver connectionStringResolver,
            IUnitOfWorkDefaultOptions defaultOptions,
            IUnitOfWorkFilterExecuter filterExecuter
            ) : base(
                connectionStringResolver,
                defaultOptions,
                filterExecuter)
        {
        }
    }
}
