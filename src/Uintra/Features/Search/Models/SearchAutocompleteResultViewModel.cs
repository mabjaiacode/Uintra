﻿using Uintra.Features.Links.Models;

namespace Uintra.Features.Search.Models
{
    public class SearchAutocompleteResultViewModel
    {
        public string Title { get; set; }
              
        public UintraLinkModel Url { get; set; }

        public SearchBoxAutocompleteItemViewModel Item { get; set; }
    }
}