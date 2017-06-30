﻿using System.Web.Mvc;
using AutoMapper;
using Compent.uIntra.Core.Bulletins;
using uIntra.Bulletins;
using uIntra.Bulletins.Web;
using uIntra.CentralFeed;
using uIntra.Core.Activity;
using uIntra.Core.Extentions;
using uIntra.Core.Media;
using uIntra.Core.User;

namespace Compent.uIntra.Controllers
{
    public class BulletinsController : BulletinsControllerBase
    {
        protected override string DetailsViewPath => "~/Views/Bulletins/DetailsView.cshtml";
        protected override string ItemViewPath => "~/Views/Bulletins/ItemView.cshtml";

        public BulletinsController(
            IBulletinsService<Bulletin> bulletinsService,
            IMediaHelper mediaHelper,
            IIntranetUserService<IIntranetUser> intranetUserService,
            IIntranetUserContentHelper intranetUserContentHelper,
            IActivityTypeProvider activityTypeProvider)
            : base(bulletinsService, mediaHelper, intranetUserService, intranetUserContentHelper, activityTypeProvider)
        {
        }

        protected override BulletinViewModel GetViewModel(BulletinBase bulletin)
        {
            var extendedBullet = (Bulletin)bulletin;
            var extendedModel = base.GetViewModel(bulletin).Map<BulletinExtendedViewModel>();
            extendedModel = Mapper.Map(extendedBullet, extendedModel);
            return extendedModel;
        }

        public ActionResult CentralFeedItem(ICentralFeedItem item)
        {
            FillLinks();
            var activity = item as Bulletin;
            var extendedModel = GetItemViewModel(activity).Map<BulletinExtendedItemViewModel>();
            extendedModel.LikesInfo = activity;
            return PartialView(ItemViewPath, extendedModel);
        }

    }
}