﻿using Uintra20.Core.Activity;

namespace Uintra20.Features.CentralFeed.Enums
{
    public enum CentralFeedTypeEnum
    {
        All = 0,
        News = IntranetActivityTypeEnum.News,
        Events = IntranetActivityTypeEnum.Events,
        Bulletins = IntranetActivityTypeEnum.Bulletins,
        PagePromotion = IntranetActivityTypeEnum.PagePromotion
    }
}