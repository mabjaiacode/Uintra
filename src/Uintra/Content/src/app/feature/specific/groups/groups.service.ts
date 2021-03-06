import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { CookieService } from 'ngx-cookie-service';
import { IGroupsData, IGroupDetailsHeaderData, IGroupModel, IBreadcrumbsItem } from './groups.interface';
import { groupsApi } from 'src/app/shared/constants/general/general.const';
import { IULink } from 'src/app/shared/interfaces/general.interface';

@Injectable({
  providedIn: "root"
})
export class GroupsService {
  readonly openStateProperty = "nav-group-links-open";

  constructor(private http: HttpClient, private cookieService: CookieService) {}

  createGroup(groupCreateModel: IGroupModel): Observable<IULink> {
    return this.http.post<IULink>(groupsApi + '/Create', groupCreateModel)
  }

  editGroup(groupEditModel: IGroupModel): Observable<IULink> {
    return this.http.post<IULink>(groupsApi + '/Edit', groupEditModel)
  }

  hideGroup(id: string): Observable<IULink> {
    return this.http.post<IULink>(groupsApi + `/Hide?id=${id}`, {});
  }

  getGroups(isMyGroups: boolean, pageNumber: number) {
    return this.http.get(groupsApi + `/List?isMyGroupsPage=${isMyGroups}&page=${pageNumber}`).toPromise();
  }

  toggleSubscribe(groupId: string) {
    return this.http.post(groupsApi + `/subscribe?groupId=${groupId}`, {}).toPromise();
  }

  getBreadcrumbs(): Observable<IBreadcrumbsItem[]> {
    return this.http.get<IBreadcrumbsItem[]>("/ubaseline/api/intranetNavigation/Breadcrumbs");
  }

  setOpenState(openState: boolean = false): void {
    this.cookieService.set(this.openStateProperty, openState.toString());
  }

  getOpenState(): boolean {
    const cookieData = this.cookieService.get(this.openStateProperty);
    return cookieData === "true";
  }
}
