import { Component, OnInit, Input, Attribute } from '@angular/core';
import { TITLE_MAX_LENGTH } from 'src/app/constants/activity/create/activity-create-const';
import { GroupsService } from 'src/app/feature/project/specific/groups/groups.service';
import { finalize } from 'rxjs/operators';
import { MAX_FILES_FOR_SINGLE } from 'src/app/constants/dropzone/drop-zone.const';
import { IMedia } from '../../activity/activity.interfaces';

export interface IEditGroupData {
  id?: string;
  title?: string;
  description?: string;
  media?: {0: string};
  mediaPreview?: {
    medias: Array<IMedia>;
  }
}

@Component({
  selector: 'groups-form',
  templateUrl: './groups-form.component.html',
  styleUrls: ['./groups-form.component.less']
})
export class GroupsFormComponent {
  @Input() data: any;
  @Input('edit') edit: any;
  title: string = "";
  description: string = "";
  medias: IMedia[] = [];
  isShowValidation: boolean = false;
  inProgress: boolean = false;
  TITLE_MAX_LENGTH: number = TITLE_MAX_LENGTH;
  //File it's array where file[0] is file's object generated by dropzone and file[1] is id
  files: any[] = [];

  constructor(
    private groupsService: GroupsService
  ) { }

  ngOnInit() {
    this.edit = this.edit !== undefined;
    this.setDataToEdit();
  }

  get isSubmitDisabled() {
    return this.inProgress;
  }

  onUploadSuccess(fileArray: Array<any> = []): void {
    this.files.push(fileArray);
  }

  onFileRemoved(removedFile: object) {
    this.files = this.files.filter(file => file[0] !== removedFile);
  }

  onImageRemove() {
    this.medias = [];
  }

  onSubmit() {
    if (this.validate()) {
      this.inProgress = true;
      const groupModel = {
        title: this.title,
        description: this.description,
        newMedia: this.getMediaIdsForResponse(),
        media: null,
        id: this.data ? this.data.id : null,
      }

      if (!this.edit) {
        this.groupsService.createGroup(groupModel).pipe(
          finalize(() => this.inProgress = false)
        ).subscribe(res => {});
      } else {
        if (this.data && this.data.media && this.data.media.length) {
          groupModel.media = this.medias[0];
        }
        this.groupsService.editGroup(groupModel).pipe(
          finalize(() => this.inProgress = false)
        ).subscribe(res => {});
      }
    }
  }

  onHide() {
    this.groupsService.hideGroup(this.data.id).subscribe(res => {})
  }

  validate(): boolean {
    if (this.title && this.description && this.files.length <= MAX_FILES_FOR_SINGLE) {
      return true;
    }

    this.isShowValidation = true;
    return false;
  }

  getMediaIdsForResponse(): string {
    return this.files.map(file => file[1]).join(',');
  }

  setDataToEdit() {
    if (this.data) {
      this.title = this.data.title;
      this.description = this.data.description;
      this.medias = Object.values(this.data.mediaPreview.medias);
    }
  }

  getTitleValidationState() {
    return (this.isShowValidation && !this.title) || (this.isShowValidation && this.title.length > TITLE_MAX_LENGTH)
  }
}
