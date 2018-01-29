using System;
using System.Collections.Generic;

namespace Wind.iSeller.Framework.Core.Application.Services.Dto
{
    /// <summary>
    /// Implements <see cref="IListResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the items in the <see cref="Items"/> list</typeparam>
    [Serializable]
    public class ListResultDto<T> : IListResult<T>
    {
        private IList<T> _items;

        /// <summary>
        /// List of items.
        /// </summary>
        public IList<T> Items
        {
            get { return _items ?? (_items = new List<T>()); }
            set { _items = value; }
        }

        /// <summary>
        /// Creates a new <see cref="ListResultDto{T}"/> object.
        /// </summary>
        public ListResultDto()
        {
            
        }

        /// <summary>
        /// Creates a new <see cref="ListResultDto{T}"/> object.
        /// </summary>
        /// <param name="items">List of items</param>
        public ListResultDto(IList<T> items)
        {
            Items = items;
        }
    }
}