﻿using System.Collections.Generic;
using System.Linq;

namespace uCommunity.Core.App_Plugins.Core.Caching.Models
{
    public class CachedList<T> : CachedItemBase
    {
        public IEnumerable<T> Items { get; set; }

        public CachedList()
        {
            Items = Enumerable.Empty<T>();
        }
    }
}