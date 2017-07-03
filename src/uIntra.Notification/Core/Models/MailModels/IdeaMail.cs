﻿using System;
using System.Collections.Generic;
using EmailWorker.Data.Model;
using uIntra.Notification.Configuration;
using uIntra.Notification.Constants;

namespace uIntra.Notification.MailModels
{
    public class IdeaMail : EmailBase
    {
        private readonly string _xpath;

        public IdeaMail(string xpath)
        {
            _xpath = xpath;
        }

        public string FullName { get; set; }

        protected override string GetXPath()
        {
            return _xpath;
        }

        public override Enum MailTemplateTypeEnum => NotificationTypeEnum.Idea;

        protected override Dictionary<string, string> GetExtraTokens()
        {
            var result = base.GetExtraTokens();
            result.Add(EmailTokensConstants.FullName, FullName);

            return result;
        }
    }
}