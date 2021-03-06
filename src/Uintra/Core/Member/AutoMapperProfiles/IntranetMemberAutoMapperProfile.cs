﻿using Compent.Extensions;
using System.Linq;
using Uintra.Core.Member.Abstractions;
using Uintra.Core.Member.Models;
using Uintra.Core.Member.Models.Dto;
using Uintra.Core.Member.Profile.Models;
using Uintra.Features.UserList.Models;

namespace Uintra.Core.Member.AutoMapperProfiles
{
    public class IntranetMemberAutoMapperProfile : AutoMapper.Profile
    {
	    public IntranetMemberAutoMapperProfile()
        {
            CreateMap<IIntranetMember, ProfileViewModel>()
                .ForMember(dst => dst.EditingMember, o => o.MapFrom(user => user))
                .ForMember(dst => dst.Photo, o => o.MapFrom(user => user.Photo.HasValue() ? user.Photo : string.Empty))
                .ForMember(dst => dst.PhotoId, o => o.MapFrom(user => user.PhotoId))
                .ForMember(dst => dst.AllowedMediaExtensions, o => o.Ignore());
            //TODO How to handle Allowed Media Extensions?
            CreateMap<IIntranetMember, MemberViewModel>()
                .ForMember(dst => dst.Id, o => o.MapFrom(user => user.Id))
                .ForMember(dst => dst.DisplayedName, o => o.MapFrom(user => user.DisplayedName))
                .ForMember(dst => dst.Email, o => o.MapFrom(user => user.Email))
                .ForMember(dst => dst.LoginName, o => o.MapFrom(user => user.LoginName))
                .ForMember(dst => dst.Photo, o => o.MapFrom(user => user.Photo))
                .ForMember(dst => dst.ProfileLink, o => o.Ignore())
                .ForMember(dst => dst.PhotoId, o => o.MapFrom(user => user.PhotoId));

            CreateMap<IIntranetMember, MemberModel>()
                .ForMember(dst => dst.Member, o => o.MapFrom(user => user))
                .ForMember(dst => dst.ProfileUrl, o => o.Ignore())
                .ForMember(dst => dst.IsGroupAdmin, o => o.Ignore())
                .ForMember(dst => dst.IsCreator, o => o.Ignore());
            
            CreateMap<Google.Apis.Admin.Directory.directory_v1.Data.User, CreateMemberDto>()
                .ForMember(dst => dst.FirstName, o => o.Ignore())
                .ForMember(dst => dst.LastName, o => o.Ignore())
                .ForMember(dst => dst.Phone, o => o.MapFrom(s => s.Phones.Any() ? s.Phones.First().Value : string.Empty))
                .ForMember(dst => dst.Department, o => o.Ignore())
                .ForMember(dst => dst.Email, o => o.MapFrom(s => s.Emails.First().Address))
                .ForMember(dst => dst.MediaId, o => o.Ignore())
                .AfterMap((src, dst) =>
                {
                    if (src.Name.FullName.Contains(' '))
                    {
                        var names = src.Name.FullName.Split(' ');

                        dst.FirstName = names[0];
                        dst.LastName = names[1];
                    }
                });

            CreateMap<Google.Apis.Admin.Directory.directory_v1.Data.User, UpdateMemberDto>()
                .ForMember(dst => dst.FirstName, o => o.Ignore())
                .ForMember(dst => dst.LastName, o => o.Ignore())
                .ForMember(dst => dst.Phone, o => o.MapFrom(s => s.Phones.Any() ? s.Phones.First().Value : string.Empty))
                .ForMember(dst => dst.Department, o => o.Ignore())
                .ForMember(dst => dst.NewMedia, o => o.Ignore())
                .ForMember(dst => dst.DeleteMedia, o => o.Ignore())
                .ForMember(dst => dst.Id, o => o.Ignore())
                .AfterMap((src, dst) =>
                {
                    if (src.Name.FullName.Contains(' '))
                    {
                        var names = src.Name.FullName.Split(' ');

                        dst.FirstName = names[0];
                        dst.LastName = names[1];
                    }
                });

            CreateMap<IIntranetMember, UpdateMemberDto>()
                .ForMember(dst => dst.DeleteMedia, o => o.Ignore())
                .ForMember(dst => dst.NewMedia, o => o.Ignore());

            //Mapper.CreateMap<SearchableMember, MentionUserModel>()
            //    .ForMember(dst => dst.Id, o => o.MapFrom(u => Guid.Parse(u.Id.ToString())))
            //    .ForMember(dst => dst.Value, o => o.MapFrom(u => u.FullName))
            //    .ForMember(dst => dst.Url, o => o.Ignore());
        }
    }
}