﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Hood.Models
{
    public class ContentListModel : IContentModel
    {
        // IContentView
        public ContentType Type { get; set; }
        public string Category { get; set; }
        public PagedList<Content> Recent { get; set; }
        public IEnumerable<ContentCategory> Categories { get; set; }

        public string Search { get; set; }
        public string Sort { get; set; }

        // List
        public PagedList<Content> Posts { get; set; }
        public ApplicationUser Author { get; set; }
        public ListFilters Filters { get; internal set; }
    }
}
