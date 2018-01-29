namespace Wind.iSeller.Framework.Core.Application.Services.Dto
{
    /// <summary>
    /// This interface is defined to standardize to request a paged and sorted result.
    /// </summary>
    public interface IPagedAndSortedResultRequest : IPagedResultRequest, ISortedResultRequest
    {
        
    }
}