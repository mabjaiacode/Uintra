﻿using Uintra.Core.Caching;
using Uintra.Core.UmbracoEventServices;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Uintra.Core.Permissions.UmbracoEvents
{
    public class UmbracoMemberGroupEventService : IUmbracoMemberGroupDeletingEventService
    {
        private readonly IBasePermissionsService _basePermissionsService;
        private readonly ICacheService _cacheService;

        public UmbracoMemberGroupEventService(IBasePermissionsService basePermissionsService,
            ICacheService cacheService)
        {
            _basePermissionsService = basePermissionsService;
            _cacheService = cacheService;
        }

        public void ProcessMemberGroupDeleting(IMemberGroupService sender, DeleteEventArgs<IMemberGroup> e)
        {
            foreach (var group in e.DeletedEntities)
            {
                _basePermissionsService.DeletePermissionsForMemberGroup(group.Id);
            }
        }
    }
}