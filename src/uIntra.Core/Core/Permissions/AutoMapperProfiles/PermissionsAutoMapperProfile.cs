﻿using System.Linq;
using AutoMapper;
using Uintra.Core.Extensions;
using Uintra.Core.Permissions.Models;
using Umbraco.Core.Models;
using static LanguageExt.Prelude;

namespace Uintra.Core.Permissions
{
    public class PermissionsAutoMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<IMemberGroup, IntranetMemberGroup>()
                .ForMember(dst => dst.Id, o => o.MapFrom(el => el.Id))
                .ForMember(dst => dst.Name, o => o.MapFrom(el => el.Name));

            Mapper.CreateMap<IntranetMemberGroup, MemberGroupViewModel>()
                .ForMember(dst => dst.Id, o => o.MapFrom(el => el.Id))
                .ForMember(dst => dst.Name, o => o.MapFrom(el => el.Name));

            Mapper.CreateMap<BasePermissionModel, PermissionViewModel>()
                .ForMember(dst => dst.ActionId, o => o.MapFrom(el => el.ActionType.ToInt()))
                .ForMember(dst => dst.ActionName, o => o.MapFrom(el => el.ActionType.ToString()))
                .ForMember(dst => dst.ActivityTypeId, o => o.MapFrom(el => el.ActivityType.ToNullableInt()))
                .ForMember(dst => dst.ActivityTypeName, o => o.MapFrom(el => el.ActivityType.Map(toString).FirstOrDefault()))
                .ForMember(dst => dst.Allowed, o => o.MapFrom(el => el.IsAllowed))
                .ForMember(dst => dst.Enabled, o => o.MapFrom(el => el.IsEnabled))
                .ForMember(dst => dst.IntranetMemberGroupId, o => o.MapFrom(el => el.Group.Id));
        }
    }
}