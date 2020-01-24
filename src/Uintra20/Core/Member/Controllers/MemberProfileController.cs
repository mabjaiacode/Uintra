﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using UBaseline.Core.Controllers;
using Uintra20.Core.Member.Entities;
using Uintra20.Core.Member.Models.Dto;
using Uintra20.Core.Member.Profile.Edit.Models;
using Uintra20.Core.Member.Services;
using Uintra20.Features.Media;
using Uintra20.Features.Notification.Configuration;
using Uintra20.Features.Notification.Services;
using Uintra20.Infrastructure.Extensions;

namespace Uintra20.Core.Member.Controllers
{
    public class MemberProfileController : UBaselineApiController
    {
        private readonly IIntranetMemberService<IntranetMember> _intranetMemberService;
        private readonly IMemberNotifiersSettingsService _memberNotifiersSettingsService;
        private readonly IMediaHelper _mediaHelper;

        public MemberProfileController(
            IIntranetMemberService<IntranetMember> intranetMemberService,
            IMemberNotifiersSettingsService memberNotifiersSettingsService,
            IMediaHelper mediaHelper)
        {
            _mediaHelper = mediaHelper;
            _intranetMemberService = intranetMemberService;
            _memberNotifiersSettingsService = memberNotifiersSettingsService;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetCurrentUserProfile()
        {
            var member = await _intranetMemberService.GetCurrentMemberAsync();

            if (member == null) return BadRequest();

            var result = member.Map<ProfileEditModel>();

            result.MemberNotifierSettings = _memberNotifiersSettingsService.GetForMember(member.Id);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IHttpActionResult> Edit(ProfileEditModel model)
        {
            var newMedias = _mediaHelper.CreateMedia(model).ToArray();

            var member = model.Map<UpdateMemberDto>();

            member.NewMedia = newMedias.Length > 0
                ? newMedias.First()
                : default(int?);

            await _intranetMemberService.UpdateAsync(member);

            return Ok();
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdateNotifierSettings(NotifierTypeEnum type, bool isEnabled)
        {
            var currentMember = await _intranetMemberService.GetCurrentMemberAsync();

            await _memberNotifiersSettingsService.SetForMemberAsync(currentMember.Id, type, isEnabled);

            return Ok();
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeletePhoto(int photoId)
        {
            var member = _intranetMemberService.GetCurrentMember();

            _mediaHelper.DeleteMedia(photoId);

            var updateUser = member.Map<UpdateMemberDto>();

            updateUser.DeleteMedia = true;

            await _intranetMemberService.UpdateAsync(updateUser);

            return Ok();
        }
    }
}