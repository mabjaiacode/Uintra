﻿using System.Collections.Generic;
using uCommunity.Events;
using uCommunity.Tagging;

namespace Compent.uIntra.Core.Events
{
    public class EventExtendedCreateModel : EventCreateModel, ITagsActivityCreateEditModel
    {
        public EventExtendedCreateModel()
        {
            Tags = new List<TagEditModel>();
        }

        public IList<TagEditModel> Tags { get; set; }
    }
}