﻿using UBaseline.Shared.Node;
using UBaseline.Shared.PageSettings;
using UBaseline.Shared.PanelContainer;
using UBaseline.Shared.Property;

namespace Uintra.Features.Social.Models
{
    public class SocialEditPageModel : NodeModel
    {
        public PropertyModel<PanelContainerModel> Panels { get; set; }
        public PageSettingsCompositionModel PageSettings { get; set; }
    }
}