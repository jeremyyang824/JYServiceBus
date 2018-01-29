using System;

namespace Wind.iSeller.Framework.Core.Application.Services.Dto
{
    /// <summary>
    /// Simply implements <see cref="IPagedResultRequest"/>.
    /// </summary>
    [Serializable]
    public class PagedResultRequestDto : LimitedResultRequestDto, IPagedResultRequest
    {
        public virtual int SkipCount { get; set; }
    }
}