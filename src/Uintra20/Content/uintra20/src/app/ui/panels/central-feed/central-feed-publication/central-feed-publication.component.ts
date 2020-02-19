import { Component, Input, OnInit, HostListener } from "@angular/core";
import { ILikeData } from "../../../../feature/project/reusable/ui-elements/like-button/like-button.interface";
import { Router } from "@angular/router";
import { ImageGalleryService } from "src/app/feature/project/reusable/ui-elements/image-gallery/image-gallery.service";
import { DomSanitizer } from "@angular/platform-browser";
import { MqService } from "src/app/services/general/mq.service";
import { IMedia, IDocument } from 'src/app/feature/project/specific/activity/activity.interfaces';

@Component({
  selector: "app-central-feed-publication",
  templateUrl: "./central-feed-publication.component.html",
  styleUrls: ["./central-feed-publication.component.less"]
})
export class CentralFeedPublicationComponent implements OnInit {
  @Input() publication;
  @HostListener("window:resize", ["$event"])
  getScreenSize(event?) {
    this.deviceWidth = window.innerWidth;
    this.countToDisplay =
      this.medias.length > 2
        ? this.getItemsCountToDisplay()
        : this.medias.length;
    this.additionalImages = this.medias.length - this.countToDisplay;
  }
  deviceWidth: number;
  documentsCount: any;
  additionalImages: number;
  countToDisplay: number;

  medias: Array<IMedia> = new Array<IMedia>();
  documents: Array<IDocument> = new Array<IDocument>();

  get commentsCount() {
    return this.publication.activity.commentsCount || "Comment";
  }

  likeData: ILikeData;

  constructor(
    private imageGalleryService: ImageGalleryService,
    private router: Router,
    private sanitizer: DomSanitizer,
    private mq: MqService
  ) {}

  ngOnInit(): void {
    this.deviceWidth = window.innerWidth;
    this.publication.activity.description = this.sanitizer.bypassSecurityTrustHtml(this.publication.activity.description);
    this.medias = Object.values(this.publication.activity.mediaPreview.medias);
    this.countToDisplay =
      this.medias.length > 2
        ? this.getItemsCountToDisplay()
        : this.medias.length;
    this.additionalImages = this.medias.length - this.countToDisplay;
    this.documents = Object.values(
      this.publication.activity.mediaPreview.otherFiles
    );
    this.documentsCount = this.documents.length;
    this.likeData = {
      likedByCurrentUser: this.publication.activity.likedByCurrentUser,
      id: this.publication.activity.id,
      activityType: this.publication.activity.activityType,
      likes: this.publication.activity.likes
    };
  }

  public openGallery(i) {
    const items = this.medias.map(el => ({
      src: el.url,
      w: el.width,
      h: el.height
    }));

    this.imageGalleryService.open(items, i);
  }

  getPublicationDate() {
    return this.publication.activity.dates.length
      ? this.publication.activity.dates[0]
      : "";
  }

  checkForRightRoute(e) {
    if (!e.target.href) {
      this.router.navigate(["/social-details"], {
        queryParams: { id: this.publication.activity.id }
      });
    }
  }

  getItemsCountToDisplay() {
    if (!this.mq.isTablet(this.deviceWidth)) {
      return 2;
    }

    return 3;
  }
}
