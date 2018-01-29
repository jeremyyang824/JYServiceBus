using System;

namespace Wind.iSeller.NServiceBus.Expo.LegacyISellerAdapter
{
    [Serializable]
    public class ISellerPaginationInfo
    {
        /// <summary>
        /// 页码的index（学刚确认）
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 翻页方向
        /// 0:上一页
        /// 1:上下各一页
        /// 2:下一页
        /// </summary>
        public byte PageDirection { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public long Records { get; set; }
    }
}
