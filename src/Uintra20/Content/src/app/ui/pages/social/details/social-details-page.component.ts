import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { ActivatedRoute } from "@angular/router";

import ParseHelper from "src/app/shared/utils/parse.helper";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { AddButtonService } from "src/app/ui/main-layout/left-navigation/components/my-links/add-button.service";
import {
  IDocument,
  ISocialDetails,
  IUserTag,
  IMedia
} from "src/app/feature/specific/activity/activity.interfaces";
import { ILikeData } from "src/app/feature/reusable/ui-elements/like-button/like-button.interface";
import { ICommentData } from "src/app/feature/reusable/ui-elements/comments/comments.component";
import { ImageGalleryService } from "src/app/feature/reusable/ui-elements/image-gallery/image-gallery.service";
import { IGroupDetailsHeaderData } from 'src/app/feature/specific/groups/groups.interface';

@Component({
  selector: "social-details",
  templateUrl: "./social-details-page.component.html",
  styleUrls: ["./social-details-page.component.less"],
  encapsulation: ViewEncapsulation.None
})
export class SocialDetailsPanelComponent implements OnInit {
  data: any;
  details: ISocialDetails;
  tags: Array<IUserTag>;
  activityName: string;
  likeData: ILikeData;
  medias: Array<IMedia> = new Array<IMedia>();
  documents: Array<IDocument> = new Array<IDocument>();
  commentDetails: ICommentData;
  detailsDescription: SafeHtml;
  groupHeader: IGroupDetailsHeaderData;
  isGroupMember: boolean;

  constructor(
    private activatedRoute: ActivatedRoute,
    private imageGalleryService: ImageGalleryService,
    private sanitizer: DomSanitizer,
    private addButtonService: AddButtonService
  ) {
    this.activatedRoute.data.subscribe(data => {
      this.data = data;
      this.addButtonService.setPageId(data.id);
    });
  }

  public ngOnInit(): void {
    const parsedData = ParseHelper.parseUbaselineData(this.data);
    this.details = parsedData.details;
    this.commentDetails = {
      entityId: parsedData.details.id,
      entityType: parsedData.details.activityType
    };
    this.isGroupMember = parsedData.isGroupMember;
    this.groupHeader = parsedData.groupHeader;
    this.activityName = ParseHelper.parseActivityType(
      this.details.activityType
    );
    this.tags = Object.values(parsedData.tags);
    this.medias = Object.values(parsedData.details.lightboxPreviewModel.medias);
    this.documents = Object.values(
      parsedData.details.lightboxPreviewModel.otherFiles
    );
    this.likeData = {
      likedByCurrentUser: !!parsedData.likedByCurrentUser,
      id: parsedData.details.id,
      activityType: parsedData.details.activityType,
      likes: Object.values(parsedData.likes)
    };
    this.detailsDescription = this.sanitizer.bypassSecurityTrustHtml(
      this.details.description
    );
  }

  public openGallery(i) {
    const items = this.medias.map(el => ({
      src: el.url,
      w: el.width,
      h: el.height
    }));

    this.imageGalleryService.open(items, i);
  }
}