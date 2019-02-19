﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using LanguageExt;
using Uintra.Core.Caching;
using Uintra.Core.Extensions;
using Uintra.Core.Permissions.Interfaces;
using Uintra.Core.Permissions.Models;
using Uintra.Core.Permissions.Sql;
using Uintra.Core.Permissions.TypeProviders;
using Uintra.Core.Persistence;
using Uintra.Core.TypeProviders;
using static Uintra.Core.Extensions.EnumerableExtensions;
using static LanguageExt.Prelude;

namespace Uintra.Core.Permissions.Implementation
{
    public class BasePermissionsService : IBasePermissionsService
    {
        protected virtual string BasePermissionCacheKey => "BasePermissionCache";

        private readonly ISqlRepository<PermissionEntity> _permissionsRepository;
        private readonly IIntranetActionTypeProvider _intranetActionTypeProvider;
        private readonly IIntranetMemberGroupProvider _intranetMemberGroupProvider;
        private readonly IActivityTypeProvider _activityTypeProvider;
        private readonly ICacheService _cacheService;
        private readonly IPermissionSettingsSchema _permissionSettingsSchema;

        public BasePermissionsService(
            ISqlRepository<PermissionEntity> permissionsRepository,
            IIntranetActionTypeProvider intranetActionProvider,
            IIntranetMemberGroupProvider intranetMemberGroupProvider,
            IActivityTypeProvider activityTypeProvider,
            ICacheService cacheService,
            IPermissionSettingsSchema permissionSettingsSchema)
        {
            _permissionsRepository = permissionsRepository;
            _intranetActionTypeProvider = intranetActionProvider;
            _intranetMemberGroupProvider = intranetMemberGroupProvider;
            _activityTypeProvider = activityTypeProvider;
            _cacheService = cacheService;
            _permissionSettingsSchema = permissionSettingsSchema;
        }

        public IReadOnlyCollection<BasePermissionModel> GetAll()
        {
            //return _cacheService.GetOrSet(
            //    BasePermissionCacheKey,
            //    () => _permissionsRepository.GetAll().Apply(MapAll));
            return _permissionsRepository.GetAll().Apply(MapAll);
        }

        public IEnumerable<BasePermissionModel> GetForGroup(IntranetMemberGroup group)
        {
            var storedPerms = GetAll().Where(i => i.Group.Id == group.Id);

            var settings = _permissionSettingsSchema.Settings
                .Select(settingIdentity => Optional(storedPerms.FirstOrDefault(i =>
                    i.ActionType.Equals(settingIdentity.ActionType) &&
                    i.ActivityType == settingIdentity.ActivityType &&
                    i.Group.Id == group.Id))
                    .IfNone(() => _permissionSettingsSchema.GetDefault(settingIdentity, group)));

            return settings;
        }

        private ReadOnlyCollection<BasePermissionModel> MapAll(IEnumerable<PermissionEntity> entities) =>
            entities.Select(Map).ToList().AsReadOnly();

        public BasePermissionModel Save(BasePermissionUpdateModel update)
        {
            var intranetActionId = update.Action.ToInt();
            var activityTypeId = update.ActivityType.ToNullableInt();
            var storedEntity = _permissionsRepository
                //.FindOrNone(AndAlso(GroupIs(update.Group), ActionIs(update.Action)));
                .FindOrNone(i => i.IntranetMemberGroupId == update.Group.Id &&
                    i.IntranetActionId == intranetActionId &&
                    i.ActivityTypeId == activityTypeId);

            var actualEntity = storedEntity.Match(
                entity =>
                {
                    var updatedEntity = UpdateEntity(entity, update);
                    _permissionsRepository.Update(updatedEntity);
                    return updatedEntity;
                },
                () =>
                {
                    var createdEntity = CreateEntity(update);
                    _permissionsRepository.Add(createdEntity);
                    return createdEntity;
                });

            var actualMappedEntity = Map(actualEntity);
            var oldCache = _cacheService.Get<IReadOnlyList<BasePermissionModel>>(BasePermissionCacheKey) ??
                new List<BasePermissionModel>().AsReadOnly();

            _cacheService.Set(BasePermissionCacheKey, oldCache.WithUpdatedElement(e => e.Id, actualMappedEntity));

            return actualMappedEntity;
        }

        public void Delete(int memberGroupId)
        {
            _permissionsRepository.Delete(i => i.IntranetMemberGroupId == memberGroupId);
        }

        protected BasePermissionModel Map(PermissionEntity entity) =>
            BasePermissionModel.Of(
                _intranetMemberGroupProvider.IntTypeDictionary,
                _intranetActionTypeProvider.IntTypeDictionary,
                _activityTypeProvider.IntTypeDictionary,
                entity);

        public static PermissionEntity UpdateEntity(PermissionEntity entity, BasePermissionUpdateModel update)
        {
            entity.IsAllowed = update.SettingValues.IsAllowed;
            entity.IsEnabled = update.SettingValues.IsEnabled;

            return entity;
        }

        public static PermissionEntity CreateEntity(BasePermissionUpdateModel update) =>
            new PermissionEntity
            {
                Id = Guid.NewGuid(),
                IntranetMemberGroupId = update.Group.Id,
                IntranetActionId = update.Action.ToInt(),
                ActivityTypeId = update.ActivityType.ToNullableInt(),
                IsAllowed = update.SettingValues.IsAllowed,
                IsEnabled = update.SettingValues.IsEnabled
            };

        //public static Expression<Func<PermissionEntity, bool>> GroupIs(IntranetMemberGroup group)
        //{
        //    var groupId = group.Id;
        //    return entity => entity.IntranetMemberGroupId == groupId;
        //}

        //public static Expression<Func<PermissionEntity, bool>> ActionIs(Enum action)
        //{
        //    var actionId = action.ToInt();
        //    return entity => entity.IntranetMemberGroupId == actionId;
        //}
    }
}