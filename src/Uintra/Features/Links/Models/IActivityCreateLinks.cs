﻿namespace Uintra.Features.Links.Models
{
    public interface IActivityCreateLinks
    {
        UintraLinkModel Feed { get; }
        UintraLinkModel Create { get; }
        UintraLinkModel Owner { get; }
    }
}
