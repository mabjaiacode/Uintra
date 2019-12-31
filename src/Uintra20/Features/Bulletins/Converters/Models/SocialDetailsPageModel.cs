﻿using UBaseline.Shared.Node;
using UBaseline.Shared.PageSettings;
using UBaseline.Shared.PanelContainer;
using UBaseline.Shared.Property;

namespace Uintra20.Core.Bulletin.Converters.Models
{
    public class SocialDetailsPageModel : NodeModel
    {
        public PropertyModel<PanelContainerModel> Panels { get; set; }
        public PageSettingsCompositionModel PageSettings { get; set; }
    }
}