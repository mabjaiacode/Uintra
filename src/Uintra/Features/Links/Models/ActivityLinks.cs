﻿namespace Uintra.Features.Links.Models
{
    public class ActivityLinks : ActivityCreateLinks, IActivityLinks
    {
        public UintraLinkModel Details { get; set; }
        public UintraLinkModel Edit { get; set; }
    }
}