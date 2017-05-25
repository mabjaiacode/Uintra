﻿using System.Linq;
using Compent.uIntra.Core.ApplicationSettings;
using uIntra.Core.User;
using uIntra.Users;
using Umbraco.Core.Models;

namespace Compent.uIntra.Core.Helpers
{
    public class UmbracoContentHelper : IUmbracoContentHelper
    {
        private readonly IIntranetUserService<IntranetUser> _intranetUserService;
        private readonly IuIntraApplicationSettings _applicationSettings;

        public UmbracoContentHelper(IIntranetUserService<IntranetUser> intranetUserService, IuIntraApplicationSettings applicationSettings)
        {
            _intranetUserService = intranetUserService;
            _applicationSettings = applicationSettings;
        }

        public bool IsContentAvailable(IPublishedContent publishedContent)
        {
            if (_intranetUserService.GetCurrentUser().Role == IntranetRolesEnum.WebMaster)
            {
                return true;
            }

            if (_applicationSettings.NotWebMasterRoleDisabledDocumentTypes.Contains(publishedContent.DocumentTypeAlias))
            {
                return false;
            }

            return true;
        }
    }
}