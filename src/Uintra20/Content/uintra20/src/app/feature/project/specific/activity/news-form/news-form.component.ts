import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  ViewEncapsulation,
  HostListener
} from "@angular/core";
import { IPinedData } from "../pin-activity/pin-activity.component";
import { ISelectItem } from "../../../reusable/inputs/select/select.component";
import { IDatepickerData } from "../../../reusable/inputs/datepicker-from-to/datepiker-from-to.interface";
import { ITagData } from "../../../reusable/inputs/tag-multiselect/tag-multiselect.interface";
import { INewsCreateModel, IOwner } from "../activity.interfaces";
import { ILocationResult } from "../../../reusable/ui-elements/location-picker/location-picker.interface";
import { NewsFormService } from "./news-form.service";
import { PinActivityService } from '../pin-activity/pin-activity.service';
import { Router } from '@angular/router';
import { HasDataChangedService } from 'src/app/services/general/has-data-changed.service';

@Component({
  selector: "app-news-form",
  templateUrl: "./news-form.component.html",
  styleUrls: ["./news-form.component.less"],
  encapsulation: ViewEncapsulation.None
})
export class NewsFormComponent implements OnInit {
  @Input() data: INewsCreateModel;
  @Input() members: Array<any>;
  @Input() creator: IOwner;
  @Input() tags: ITagData[];

  @Output() handleSubmit = new EventEmitter();
  @Output() handleCancel = new EventEmitter();
  @HostListener('window:beforeunload') doSomething() {
    return false;
  }

  isShowValidation: boolean;
  newsData: INewsCreateModel;
  files: Array<any> = [];
  selectedTags: ITagData[] = [];
  isAccepted: boolean;
  owners: ISelectItem[];
  defaultOwner: ISelectItem;
  initialDates: {
    from: string;
    to: string;
  };
  initialLocation: string;
  routerSubscription: any;

  constructor(
    private newsFormService: NewsFormService,
    private pinActivityService: PinActivityService,
    private router: Router,
    private hasDataChangedService: HasDataChangedService
    ) {}

  ngOnInit() {
    this.newsData = this.newsFormService.getNewsDataInitialValue(this.data);
    this.setInitialData();
  }

  private setInitialData(): void {
    this.owners = this.getOwners();

    this.defaultOwner = this.creator
      ? this.owners.find(x => x.id === this.creator.id)
      : null;

    this.selectedTags = this.data.tags || [];

    this.initialDates = {
      from: this.data.publishDate || undefined,
      to: this.data.unpublishDate || undefined
    };

    this.initialLocation =
      (this.data.location && this.data.location.address) || null;

    if (this.newsData.isPinned) {
      this.isAccepted = true;
    }
  }

  get isInvalidEndPinDate() {
    return (
      this.newsData.isPinned &&
      (this.newsData.endPinDate < this.newsData.publishDate ||
        this.newsData.endPinDate > this.newsData.unpublishDate)
    );
  }

  // File functions
  onUploadSuccess(fileArray: Array<any> = []): void {
    this.files.push(fileArray);
    if (this.checkIfDataChanged()) {
      this.hasDataChangedService.onDataChanged();
    }
  }
  onFileRemoved(removedFile: object) {
    this.files = this.files.filter(file => file[0] !== removedFile);
    if (this.checkIfDataChanged()) {
      this.hasDataChangedService.onDataChanged();
    }
  }
  public handleImageRemove(image): void {
    this.newsData.media.medias = this.newsData.media.medias.filter(
      m => m !== image
    );
    if (this.checkIfDataChanged()) {
      this.hasDataChangedService.onDataChanged();
    }
  }
  public handleFileRemove(file): void {
    this.newsData.media.otherFiles = this.newsData.media.otherFiles.filter(
      m => m !== file
    );
    if (this.checkIfDataChanged()) {
      this.hasDataChangedService.onDataChanged();
    }
  }

  // Data set functions
  setDatePickerValue(value: IDatepickerData = {}) {
    this.pinActivityService.setPublishDates(value);
    this.newsData.publishDate = value.from;
    this.newsData.unpublishDate = value.to;
    // if (this.checkIfDataChanged()) {
    //   this.hasDataChangedService.onDataChanged();
    // }
  }
  setPinValue(value: IPinedData) {
    this.newsData.endPinDate = value.pinDate;
    this.newsData.isPinned = value.isPinCheked;
    this.isAccepted = value.isAccepted;
    // if (this.checkIfDataChanged()) {
    //   this.hasDataChangedService.onDataChanged();
    // }
  }
  setLocationValue(location: ILocationResult): void {
    this.newsData.location.address = location.address;
    this.newsData.location.shortAddress = location.shortAddress;
    if (this.checkIfDataChanged()) {
      this.hasDataChangedService.onDataChanged();
    }
  }

  // Main submit function
  onSubmit() {
    this.isShowValidation = true;

    if (this.validate()) {
      this.newsDataBuilder();
      this.handleSubmit.emit(this.newsData);
    } else {
      // TODO: scroll to invalid input
    }
  }

  //Main cancel function
  onCancel() {
    this.router.navigate(['/']);
  }

  private validate(): boolean {
    const pinValid = this.newsData.isPinned ? this.isAccepted : true;
    return (
      this.newsData.title &&
      this.newsData.description &&
      pinValid
      // !this.isInvalidEndPinDate
    );
  }

  checkIfDataChanged() {
    return this.newsData.ownerId !== this.defaultOwner.id
      || this.newsData.title.length
      || this.newsData.description.length
      || this.selectedTags.length
      || this.initialDates.from !== this.newsData.publishDate
      || this.initialDates.to !== this.newsData.unpublishDate
      || this.initialLocation !== this.newsData.location.address
      || this.newsData.location.shortAddress
      || this.newsData.isPinned
      || this.files.length;
  }

  private newsDataBuilder(): void {
    this.newsData.newMedia = this.getMediaIdsForResponse();
    this.newsData.tagIdsData = this.getTagsForResponse();
  }

  changeOwner(e) {
    this.newsData.ownerId = e;
    // if (this.checkIfDataChanged()) {
    //   this.hasDataChangedService.onDataChanged();
    // }
  }

  // TODO: move to service
  private getTagsForResponse(): string[] {
    return this.selectedTags ? this.selectedTags.map(tag => tag.id) : [];
  }
  private getMediaIdsForResponse(): string {
    return this.files.map(file => file[1]).join(',');
  }
  private getOwners(): ISelectItem[] {
    const owners = this.getMembers(this.members);
    if (this.creator) {
      owners.push({
        id: this.creator.id,
        text: this.creator.displayedName
      });
    }

    return owners;
  }
  private getMembers(members = []): ISelectItem[] {
    return members.map(member => ({
      id: member.id,
      text: member.displayedName
    }));
  }
}
