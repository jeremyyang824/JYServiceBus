using System;
using NHibernate;
using Wind.iSeller.Framework.Core.Domain.Uow;
using Wind.Core.NHibernate.Uow;

namespace Wind.iSeller.Framework.NHibernate.Uow
{
    internal static class UnitOfWorkExtensions
    {
        public static ISession GetSession(this IActiveUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            if (!(unitOfWork is NhUnitOfWork))
            {
                throw new ArgumentException("unitOfWork is not type of " + typeof(NhUnitOfWork).FullName, "unitOfWork");
            }

            return (unitOfWork as NhUnitOfWork).Session;
        }
    }
}