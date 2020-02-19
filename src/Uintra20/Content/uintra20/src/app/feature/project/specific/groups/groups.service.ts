import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { CookieService } from 'ngx-cookie-service';
import { IUlinkWithTitle, IULink } from 'src/app/feature/shared/interfaces/IULink';
import { groupsApi } from 'src/app/constants/general/general.const';

export interface IGroupsData {
  groupPageItem: IUlinkWithTitle;
  items: IUlinkWithTitle[];
}
export interface IGroupModel {
  title: string;
  description: string;
  newMedia: string;
  media: string[] | null;
  id?: string;
}
export interface IGroupDetailsHeaderData {
  title: string;
  groupLinks: {
    groupRoomPage: IULink;
    groupDocumentsPage: IULink;
    groupMembersPage: IULink;
    groupEditPage?: IULink;
  }
}

@Injectable({
  providedIn: "root"
})
export class GroupsService {
  readonly openStateProperty = "nav-group-links-open";

  constructor(private http: HttpClient, private cookieService: CookieService) {}

  getGroupsLinks(): Observable<IGroupsData> {
    return this.http.get<IGroupsData>(groupsApi + `/LeftNavigation`);
  }

  getGroupDetailsLinks(id: string): Observable<IGroupDetailsHeaderData> {
    return this.http.get<IGroupDetailsHeaderData>(groupsApi + `/Header?groupId=${id}`);
  }

  createGroup(groupCreateModel: IGroupModel): Observable<IULink> {
    return this.http.post<IULink>(groupsApi + '/Create', groupCreateModel)
  }

  editGroup(groupEditModel: IGroupModel): Observable<IULink> {
    return this.http.post<IULink>(groupsApi + '/Edit', groupEditModel)
  }

  hideGroup(id: string) {
    return this.http.post<any>(groupsApi + `/Hide?groupId=${id}`, {});
  }

  getGroups(isMyGroups: boolean, pageNumber: number) {
    return this.http.get(`/ubaseline/api/Group/List?isMyGroupsPage=${isMyGroups}&page=${pageNumber}`);
  }

  setOpenState(openState: boolean = false): void {
    this.cookieService.set(this.openStateProperty, openState.toString());
  }

  getOpenState(): boolean {
    const cookieData = this.cookieService.get(this.openStateProperty);
    return cookieData === "true";
  }
}
