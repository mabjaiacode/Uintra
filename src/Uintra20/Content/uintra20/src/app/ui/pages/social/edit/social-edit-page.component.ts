import { Component, ViewEncapsulation, HostListener } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import ParseHelper from '../../../../feature/shared/helpers/parse.helper';
import { finalize } from 'rxjs/operators';
import { ActivityService } from 'src/app/feature/project/specific/activity/activity.service';
import { ISocialEdit } from 'src/app/feature/project/specific/activity/activity.interfaces';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { RouterResolverService } from 'src/app/services/general/router-resolver.service';
import { AddButtonService } from 'src/app/ui/main-layout/left-navigation/components/my-links/add-button.service';
import { Observable } from 'rxjs';
import { HasDataChangedService } from 'src/app/services/general/has-data-changed.service';
import { CanDeactivateGuard } from 'src/app/services/general/can-deactivate.service';

@Component({
  selector: 'social-edit',
  templateUrl: './social-edit-page.component.html',
  styleUrls: ['./social-edit-page.component.less'],
  encapsulation: ViewEncapsulation.None
})
export class SocialEditPageComponent {
  @HostListener('window:beforeunload') checkIfDataChanged() {
    return !this.hasDataChangedService.hasDataChanged;
  }
  files = [];
  private data: any;
  public inProgress = false;
  public socialEdit: ISocialEdit;
  public uploadedData: Array<any> = new Array<any>();
  public socialEditForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private socialService: ActivityService,
    private router: Router,
    private routerResolverService: RouterResolverService,
    private addButtonService: AddButtonService,
    private hasDataChangedService: HasDataChangedService,
    private canDeactivateService: CanDeactivateGuard,
  ) {
    this.route.data.subscribe(data => {
      this.data = data;
      this.addButtonService.setPageId(data.id);
    });
    this.onParse();
    this.initSocialEditForm();
  }

  private onParse = (): void => {
    const parsedSocialEdit = ParseHelper.parseUbaselineData(this.data);
    // TODO: Imvestigate about parsing ubaseline data
    this.socialEdit = {
      ownerId: parsedSocialEdit.ownerId,
      description: parsedSocialEdit.description,
      tags: Object.values(parsedSocialEdit.tags),
      availableTags: Object.values(parsedSocialEdit.availableTags),
      lightboxPreviewModel: {
        medias: Object.values(parsedSocialEdit.lightboxPreviewModel.medias || []),
        otherFiles: Object.values(parsedSocialEdit.lightboxPreviewModel.otherFiles || []),
        filesToDisplay: parsedSocialEdit.lightboxPreviewModel.filesToDisplay,
        additionalImages: parsedSocialEdit.lightboxPreviewModel.additionalImages,
        hiddenImagesCount: parsedSocialEdit.lightboxPreviewModel.hiddenImagesCount
      },
      id: parsedSocialEdit.id,
      groupId: parsedSocialEdit.groupId,
      links: parsedSocialEdit.links,
      name: parsedSocialEdit.name,
      tagIdsData: new Array<string>(),
      newMedia: null,
      media: null,
      mediaRootId: parsedSocialEdit.mediaRootId
    };
  }

  public handleImageRemove(image): void {
    this.socialEdit.lightboxPreviewModel.medias =
      this.socialEdit.lightboxPreviewModel.medias.filter(m => m !== image);
    this.hasDataChangedService.onDataChanged();

  }

  public handleFileRemove(file): void {
    this.socialEdit.lightboxPreviewModel.otherFiles =
      this.socialEdit.lightboxPreviewModel.otherFiles.filter(m => m !== file);
    this.hasDataChangedService.onDataChanged();
  }

  public handleUpload(file: Array<object>): void {
    this.uploadedData.push(file);
    this.hasDataChangedService.onDataChanged();
  }

  public handleRemove(file: object): void {
    this.uploadedData = this.uploadedData.filter(d => d[0] !== file);
  }

  onTagsChange(e) {
    if (this.socialEdit.tags != e) {
      this.hasDataChangedService.onDataChanged();
    }
    this.socialEdit.tags = e;
  }

  onDescriptionChange(e) {
    if (this.socialEdit.description != e) {
      this.hasDataChangedService.onDataChanged();
    }
    this.socialEdit.description = e;
  }

  public handleSocialUpdate(): void {
    this.socialEdit.media = '';

    const otherFilesIds = this.socialEdit.lightboxPreviewModel.otherFiles
      .map(m => m.id);
    const mediaIds = this.socialEdit.lightboxPreviewModel.medias
      .map(m => m.id);

    this.socialEdit.media = otherFilesIds.concat(mediaIds).join(',');
    this.socialEdit.newMedia = this.uploadedData.map(u => u[1]).join(',');
    this.socialEdit.tagIdsData = this.socialEdit.tags.map(t => t.id);
    this.inProgress = true;

    this.socialService.updateSocial(this.socialEdit)
      .pipe(finalize(() => this.inProgress = false))
      .subscribe(
        (next: any) => {
          this.routerResolverService.removePageRouter(next.originalUrl);
          this.hasDataChangedService.reset();
          this.router.navigate([next.originalUrl]);
        },
      );
  }

  // TODO: Add usage of alertify or smth similiar
  public handleSocialDelete(): void {
    this.inProgress = true;
    this.socialService.deleteSocial(this.socialEdit.id)
      .pipe(finalize(() => this.inProgress = false))
      .subscribe(
        (next) => {
          // this.router.navigate(['/socials']); // TODO: socials doesnt exist, uncomment code when it will be done.
        },
      );
  }
  private initSocialEditForm(): void {
    this.socialEditForm = new FormGroup({
      description: new FormControl(this.socialEdit.description, Validators.required)
    });
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (this.hasDataChangedService.hasDataChanged) {
      this.canDeactivateService.canDeacrivateConfirm();
    }

    return true;
  }
}
