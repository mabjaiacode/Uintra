import { Component, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { GroupsService } from 'src/app/ui/main-layout/left-navigation/components/groups/groups.service';
import { MAX_FILES_FOR_SINGLE } from 'src/app/constants/dropzone/drop-zone.const';
import { TITLE_MAX_LENGTH } from 'src/app/constants/activity/create/activity-create-const'

@Component({
  selector: 'uintra-groups-create-page',
  templateUrl: './uintra-groups-create-page.html',
  styleUrls: ['./uintra-groups-create-page.less'],
  encapsulation: ViewEncapsulation.None
})
export class UintraGroupsCreatePage {
  data: any;
  title: string = "";
  description: string = "";
  files: any[] = [];
  isShowValidation: boolean = false;
  inProgress: boolean = false;
  TITLE_MAX_LENGTH: number = TITLE_MAX_LENGTH;

  constructor(
    private route: ActivatedRoute,
    private groupsService: GroupsService
  ) {
    this.route.data.subscribe(data => this.data = data);
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

  onSubmit() {
    if (this.validate()) {
      this.inProgress = true;

      const groupCreateModel = {
        title: this.title,
        description: this.description,
        newMedia: this.getMediaIdsForResponse(),
        media: null
      }

    this.groupsService.createGroup(groupCreateModel).pipe(
        finalize(() => this.inProgress = false)
      ).subscribe(res => {});
    }
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
}
