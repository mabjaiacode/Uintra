﻿using System;
using Compent.CommandBus;
using Uintra.Core.Activity;
using Uintra.Features.Media.Intranet.Services.Contracts;
using Uintra.Features.Media.Video.Commands;

namespace Uintra.Features.Social.Handlers
{
    public class SocialHandler : IHandle<VideoConvertedCommand> 
    {
        private readonly IIntranetMediaService _intranetMediaService;
        private readonly IIntranetActivityService<Entities.Social> _intranetActivityService;

        public SocialHandler(
            IIntranetMediaService intranetMediaService, 
            IIntranetActivityService<Entities.Social> intranetActivityService)
        {
            _intranetMediaService = intranetMediaService;
            _intranetActivityService = intranetActivityService;
        }

        public BroadcastResult Handle(VideoConvertedCommand command)
        {
            var entityId = _intranetMediaService.GetEntityIdByMediaId(command.MediaId);

            var entity = _intranetActivityService.Get(entityId);

            if (entity == null) return BroadcastResult.Success;

            entity.ModifyDate = DateTime.UtcNow;

            return BroadcastResult.Success;
        }
    }
}