﻿using System;
using Uintra.Core.Activity;
using Uintra.Infrastructure.Constants;
using Uintra.Infrastructure.Extensions;

namespace Uintra.Infrastructure.Providers
{
    public class DocumentTypeProvider : IDocumentTypeAliasProvider
    {
        public virtual string GetNavigationComposition() => DocumentTypeAliasConstants.NavigationComposition;

        public virtual string GetArticlePage() => DocumentTypeAliasConstants.ArticlePage;

        public virtual string GetHeading() => DocumentTypeAliasConstants.Heading;

        public virtual string GetSearchResultPage() => DocumentTypeAliasConstants.SearchResultPage;

        public virtual string GetProfilePage() => DocumentTypeAliasConstants.ProfilePage;

        public virtual string GetProfileEditPage() => DocumentTypeAliasConstants.ProfileEditPage;

        public virtual string GetNotificationPage() => DocumentTypeAliasConstants.NotificationPage;

        public virtual string GetHomePage() => DocumentTypeAliasConstants.HomePage;

        public virtual string GetErrorPage() => DocumentTypeAliasConstants.ErrorPage;

        public virtual string GetBulletinsDetailsPage() => DocumentTypeAliasConstants.SocialDetailsPage;

        public virtual string GetEventsDetailsPage() => DocumentTypeAliasConstants.EventsDetailsPage;

        public virtual string GetNewsDetailsPage() => DocumentTypeAliasConstants.NewsDetailsPage;

        public virtual string GetOverviewPage(Enum type)
        {
            switch (type.ToInt())
            {
                case (int) IntranetActivityTypeEnum.News: return DocumentTypeAliasConstants.NewsOverviewPage;
                case (int) IntranetActivityTypeEnum.Events: return DocumentTypeAliasConstants.EventsOverviewPage;
                case (int) IntranetActivityTypeEnum.Social: return DocumentTypeAliasConstants.SocialOverviewPage;
                default:
                    return null;
            }
        }

        public virtual string GetEditPage(Enum type)
        {
            switch (type.ToInt())
            {
                case (int) IntranetActivityTypeEnum.News: return DocumentTypeAliasConstants.NewsEditPage;
                case (int) IntranetActivityTypeEnum.Events: return DocumentTypeAliasConstants.EventsEditPage;
                case (int) IntranetActivityTypeEnum.Social: return DocumentTypeAliasConstants.SocialEditPage;
                default:
                    return null;
            }
        }

        public virtual string GetDetailsPage(Enum type)
        {
            switch (type.ToInt())
            {
                case (int) IntranetActivityTypeEnum.News: return DocumentTypeAliasConstants.NewsDetailsPage;
                case (int) IntranetActivityTypeEnum.Events: return DocumentTypeAliasConstants.EventsDetailsPage;
                case (int) IntranetActivityTypeEnum.Social: return DocumentTypeAliasConstants.SocialDetailsPage;
                default:
                    return null;
            }
        }

        public virtual string GetCreatePage(Enum type)
        {
            switch (type.ToInt())
            {
                case (int) IntranetActivityTypeEnum.News: return DocumentTypeAliasConstants.NewsCreatePage;
                case (int) IntranetActivityTypeEnum.Events: return DocumentTypeAliasConstants.EventsCreatePage;
                default:
                    return null;
            }
        }

        public virtual string GetMailTemplateFolder() => DocumentTypeAliasConstants.MailTemplatesFolder;

        public virtual string GetMailTemplate() => DocumentTypeAliasConstants.MailTemplate;

        public virtual string GetDataFolder() => DocumentTypeAliasConstants.DataFolder;

        public virtual string GetUserTagFolder() => DocumentTypeAliasConstants.UserTagFolder;

        public virtual string GetUserTag() => DocumentTypeAliasConstants.UserTag;

        public virtual string GetSystemLink() => DocumentTypeAliasConstants.SystemLink;

        public virtual string GetSystemLinkFolder() => DocumentTypeAliasConstants.SystemLinkFolder;

        public virtual string GetGroupOverviewPage() => DocumentTypeAliasConstants.GroupsOverviewPage;

        public virtual string GetGroupCreatePage() => DocumentTypeAliasConstants.GroupsCreatePage;

        public virtual string GetGroupRoomPage() => DocumentTypeAliasConstants.GroupsRoomPage;

        public virtual string GetGroupEditPage() => DocumentTypeAliasConstants.GroupsEditPage;

        public virtual string GetGroupMyGroupsOverviewPage() => DocumentTypeAliasConstants.GroupsMyGroupsOverviewPage;

        public virtual string GetGroupDeactivatedPage() => DocumentTypeAliasConstants.GroupsDeactivatedGroupPage;
    }
}