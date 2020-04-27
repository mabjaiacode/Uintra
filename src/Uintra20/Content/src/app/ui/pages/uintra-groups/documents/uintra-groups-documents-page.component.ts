import { Component, ViewEncapsulation, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { UintraGroupsService } from "./uintra-groups-documents-page.service";
import { DropzoneWrapperComponent } from 'src/app/feature/reusable/ui-elements/dropzone-wrapper/dropzone-wrapper.component';
import { UintraGroupsDocuments } from 'src/app/shared/interfaces/pages/uintra-groups/documents/uintra-groups-documents.interface';

@Component({
  selector: "uintra-groups-documents-page",
  templateUrl: "./uintra-groups-documents-page.html",
  styleUrls: ["./uintra-groups-documents-page.less"],
  encapsulation: ViewEncapsulation.None
})
export class UintraGroupsDocumentsPage {
  @ViewChild(DropzoneWrapperComponent, { static: false })
  public dropdownRef: DropzoneWrapperComponent;
  public data: UintraGroupsDocuments;
  // File it's array where file[0] is file's object generated by dropzone and file[1] is id
  public files: any[] = [];
  public isUploadPossible: boolean;

  constructor(
    private route: ActivatedRoute,
    private uintraGroupsService: UintraGroupsService,
    private router: Router,
  ) {
    this.route.data.subscribe((data: UintraGroupsDocuments) => {
      if (!data.requiresRedirect) {
        this.data = data;
      } else {
        this.router.navigate([data.errorLink.originalUrl]);
      }
    });
  }

  public onUploadSuccess(fileArray: Array<any> = []): void {
    this.isUploadPossible = true;
    this.files.push(fileArray);
  }

  public onFileRemoved(removedFile: object) {
    this.files = this.files.filter(file => {
      const fileElement = file[0];
      return fileElement !== removedFile;
    });
    this.isUploadPossible = this.files.length > 0;
  }

  public onUploadFiles(): void {
    const filesId = this.getMediaIdsForResponse();

    if (filesId) {
      this.uintraGroupsService
        .uploadFile(filesId, this.data.groupId)
        .subscribe(r => {
          this.uintraGroupsService.refreshDocuments();
          this.files = [];
          this.dropdownRef.handleReset();
        });
    }
  }

  public getMediaIdsForResponse(): string {
    return this.files.map(file => file[1]).join(",");
  }
}
