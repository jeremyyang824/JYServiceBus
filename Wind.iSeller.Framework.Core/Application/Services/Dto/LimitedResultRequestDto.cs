
namespace Wind.iSeller.Framework.Core.Application.Services.Dto
{
    /// <summary>
    /// Simply implements <see cref="ILimitedResultRequest"/>.
    /// </summary>
    public class LimitedResultRequestDto : ILimitedResultRequest
    {
        public virtual int MaxResultCount { get; set; }

        public LimitedResultRequestDto()
        {
            this.MaxResultCount = 10;
        }
    }
}