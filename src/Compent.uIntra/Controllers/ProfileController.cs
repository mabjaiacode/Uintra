﻿using System;
using System.Linq;
using System.Web.Mvc;
using Compent.Uintra.Core.Users;
using Uintra.Core;
using Uintra.Core.ApplicationSettings;
using Uintra.Core.Extensions;
using Uintra.Core.Media;
using Uintra.Core.User;
using Uintra.Notification;
using Uintra.Tagging.UserTags;
using Uintra.Users;
using Uintra.Users.Web;
using Umbraco.Web;

namespace Compent.Uintra.Controllers
{
    public class ProfileController : ProfileControllerBase
    {
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IIntranetUserContentProvider _intranetUserContentProvider;
        private readonly IIntranetMemberService<IIntranetMember> _intranetMemberService;
        private readonly UserTagService _userTagService;

        protected override string ProfileEditViewPath { get; } = "~/Views/Profile/Edit.cshtml";

        public ProfileController(
            IMediaHelper mediaHelper,
            IApplicationSettings applicationSettings,
            IIntranetMemberService<IIntranetMember> intranetMemberService,
            IMemberNotifiersSettingsService memberNotifiersSettingsService,
            UmbracoHelper umbracoHelper,
            IIntranetUserContentProvider intranetUserContentProvider, UserTagService userTagService)
            : base(mediaHelper, applicationSettings, intranetMemberService, memberNotifiersSettingsService)
        {
            _umbracoHelper = umbracoHelper;
            _intranetUserContentProvider = intranetUserContentProvider;
            _userTagService = userTagService;
            _intranetMemberService = intranetMemberService;
        }

        public ActionResult EditPage()
        {
            var profilePage = _intranetUserContentProvider.GetEditPage();
            return Redirect(profilePage.Url);
        }

        public override MediaSettings GetMediaSettings()
        {
            var media = _umbracoHelper.TypedMediaAtRoot();
            var id =
                media.Single(m => m.GetPropertyValue<MediaFolderTypeEnum>(FolderConstants.FolderTypePropertyTypeAlias) ==
                                  MediaFolderTypeEnum.MembersContent).Id;

            return new MediaSettings
            {
                MediaRootId = id
            };
        }

        [NonAction]
        public override ActionResult Edit(ProfileEditModel model) => base.Edit(model);

        [HttpPost]
        public ActionResult Edit(ExtendedProfileEditModel model)
        {
            var user = MapToUserDTO(model);
            var tagIds = model.TagIdsData.ParseStringCollection(Guid.Parse);
            _userTagService.Replace(user.Id, tagIds);
            _intranetMemberService.Update(user);
            return RedirectToCurrentUmbracoPage();
        }

        [HttpGet]
        public override ActionResult Edit()
        {
            var user = _intranetMemberService.GetCurrentMember();
            var result = MapToEditModel(user);

            return PartialView(ProfileEditViewPath, result);
        }

        private new ExtendedProfileEditModel MapToEditModel(IIntranetMember member)
        {
            var baseModel = base.MapToEditModel(member);
            var result = baseModel.Map<ExtendedProfileEditModel>();
            return result;
        }
    }
}