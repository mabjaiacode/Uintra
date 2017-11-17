﻿using uIntra.Core.Extensions;

namespace uIntra.Notification.Configuration
{
    public class DefaultNotifierTemplateProvider :
        IDefaultNotifierTemplateProvider<EmailNotifierTemplate>,
        IDefaultNotifierTemplateProvider<UiNotifierTemplate>
    {
        private readonly IDefaultTemplateReader _defaultTemplateReader;

        public DefaultNotifierTemplateProvider(IDefaultTemplateReader defaultTemplateReader)
        {
            _defaultTemplateReader = defaultTemplateReader;
        }

        EmailNotifierTemplate IDefaultNotifierTemplateProvider<EmailNotifierTemplate>.GetTemplate(ActivityEventIdentity activityEvent) =>
            GetTemplate<EmailNotifierTemplate>(activityEvent, NotifierTypeEnum.EmailNotifier);

        UiNotifierTemplate IDefaultNotifierTemplateProvider<UiNotifierTemplate>.GetTemplate(ActivityEventIdentity activityEvent) =>
            GetTemplate<UiNotifierTemplate>(activityEvent, NotifierTypeEnum.UiNotifier);

        private T GetTemplate<T>(ActivityEventIdentity activityEvent, NotifierTypeEnum notifier)
        {
            var notificationType = activityEvent.AddNotifierIdentity(notifier);
            var result = _defaultTemplateReader.ReadTemplate(notificationType).Deserialize<T>();
            return result;
        }
    }
}