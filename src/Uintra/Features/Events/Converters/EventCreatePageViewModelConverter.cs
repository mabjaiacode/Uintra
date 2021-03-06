﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Compent.Extensions;
using UBaseline.Core.RequestContext;
using Uintra.Core.Activity;
using Uintra.Core.Member.Entities;
using Uintra.Core.Member.Services;
using Uintra.Core.UbaselineModels.RestrictedNode;
using Uintra.Features.Events.Entities;
using Uintra.Features.Events.Models;
using Uintra.Features.Groups.Helpers;
using Uintra.Features.Groups.Services;
using Uintra.Features.Links;
using Uintra.Features.Permissions;
using Uintra.Features.Permissions.Interfaces;
using Uintra.Features.Permissions.Models;
using Uintra.Features.Tagging.UserTags.Services;
using Uintra.Infrastructure.Extensions;

namespace Uintra.Features.Events.Converters
{
    public class EventCreatePageViewModelConverter : UintraRestrictedNodeViewModelConverter<EventCreatePageModel, EventCreatePageViewModel>
    {
        private const PermissionResourceTypeEnum PermissionType = PermissionResourceTypeEnum.Events;
        private const IntranetActivityTypeEnum ActivityType = IntranetActivityTypeEnum.Events;

        private readonly IIntranetMemberService<IntranetMember> _memberService;
        private readonly IPermissionsService _permissionsService;
        private readonly IUserTagProvider _tagProvider;
        private readonly IFeedLinkService _feedLinkService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IGroupHelper _groupHelper;
        private readonly IUBaselineRequestContext _context;
        private readonly IEventsService<Event> _eventsService;
        private readonly IGroupService _groupService;

        public EventCreatePageViewModelConverter(
            IIntranetMemberService<IntranetMember> memberService,
            IPermissionsService permissionsService,
            IUserTagProvider tagProvider,
            IFeedLinkService feedLinkService,
            IGroupMemberService groupMemberService,
            IGroupHelper groupHelper,
            IErrorLinksService errorLinksService, 
            IUBaselineRequestContext context,
            IEventsService<Event> eventsService,
            IGroupService groupService) 
            : base(errorLinksService)
        {
            _memberService = memberService;
            _permissionsService = permissionsService;
            _tagProvider = tagProvider;
            _feedLinkService = feedLinkService;
            _groupMemberService = groupMemberService;
            _groupHelper = groupHelper;
            _context = context;
            _eventsService = eventsService;
            _groupService = groupService;
        }

        public override ConverterResponseModel MapViewModel(EventCreatePageModel node, EventCreatePageViewModel viewModel)
        {
            var groupId = _context.ParseQueryString("groupId").TryParseGuid();

            if (groupId.HasValue)
            {
                var group = _groupService.Get(groupId.Value);
                if (group == null || group.IsHidden)
                    return NotFoundResult();
            }

            if (!HasPermission(groupId))
            {
                return ForbiddenResult();
            }

            viewModel.Data = GetData(groupId);
            viewModel.GroupHeader = groupId.HasValue ? _groupHelper.GetHeader(groupId.Value) : null;

            return OkResult();
        }

        private EventCreateDataViewModel GetData(Guid? groupId)
        {
            var model = new EventCreateDataViewModel {GroupId = groupId};

            var currentMember = _memberService.GetCurrentMember();

            model.CanEditOwner = _permissionsService.Check(PermissionType, PermissionActionEnum.EditOwner);
            model.PinAllowed = _permissionsService.Check(PermissionType, PermissionActionEnum.CanPin);

            if (model.CanEditOwner)
                model.Members = GetUsersWithAccess(new PermissionSettingIdentity(PermissionActionEnum.Create, PermissionType));

            model.Links = model.GroupId.HasValue ?
                _feedLinkService.GetCreateLinks(ActivityType, model.GroupId.Value)
                : _feedLinkService.GetCreateLinks(ActivityType);
            
            var mediaSettings = _eventsService.GetMediaSettings();

            model.AllowedMediaExtensions = mediaSettings.AllowedMediaExtensions;
            model.Tags = _tagProvider.GetAll();
            model.Creator = currentMember.ToViewModel();

            var now = DateTime.UtcNow;

            model.PublishDate = now;
            model.StartDate = now;
            model.EndDate = now.AddHours(8);

            return model;
        }
        
        private bool HasPermission(Guid? groupId)
        {
            var hasPermission = _permissionsService.Check(PermissionType, PermissionActionEnum.Create);

            if (groupId.HasValue)
            {
                hasPermission = hasPermission &&
                                _groupMemberService.IsGroupMember(groupId.Value, _memberService.GetCurrentMemberId());
            }

            return hasPermission;
        }

        private IEnumerable<IntranetMember> GetUsersWithAccess(PermissionSettingIdentity permissionSettingIdentity) =>
            _memberService
                .GetAll()
                .Where(member => _permissionsService.Check(member, permissionSettingIdentity))
                .OrderBy(user => user.DisplayedName)
                .ToArray();
    }
}