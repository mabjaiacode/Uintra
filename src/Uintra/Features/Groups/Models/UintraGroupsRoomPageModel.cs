﻿using UBaseline.Shared.Node;
using UBaseline.Shared.PageSettings;
using UBaseline.Shared.PanelContainer;
using UBaseline.Shared.Property;

namespace Uintra.Features.Groups.Models
{
    public class UintraGroupsRoomPageModel : NodeModel, IPanelsComposition, IGroupNavigationComposition, IPageSettingsComposition
    {
        public GroupNavigationCompositionModel GroupNavigation { get; set; }
        public PropertyModel<PanelContainerModel> Panels { get; set; }
        public PageSettingsCompositionModel PageSettings { get; set; }
    }
}