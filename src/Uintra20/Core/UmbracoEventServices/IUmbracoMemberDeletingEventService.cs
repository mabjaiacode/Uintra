﻿using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Uintra20.Core.UmbracoEventServices
{
    public interface IUmbracoMemberDeletingEventService
    {
        void ProcessMemberDeleting(IMemberService sender, DeleteEventArgs<IMember> args);
    }
}