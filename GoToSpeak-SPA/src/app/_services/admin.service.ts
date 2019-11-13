import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { Log } from '../_models/log';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }
getUsersWithRoles(page?, itemsPerPage?): Observable<PaginatedResult<User[]>> {
  const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
  let params = new HttpParams();
  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }
  return this.http.get<User[]>(this.baseUrl + 'admin/usersWithRoles', {observe: 'response', params})
  .pipe(
    map(response => {
      paginatedResult.result = response.body;
      if (response.headers.get('Pagination') != null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    })
  );
}
getLogs(page?, itemsPerPage?, logParams?): Observable<PaginatedResult<Log[]>> {
  const paginatedResult: PaginatedResult<Log[]> = new PaginatedResult<Log[]>();
  let params = new HttpParams();
  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }
  if(logParams != null) {
    params = params.append('name', logParams.userName);
    params = params.append('level', logParams.level);
    params = params.append('lastXDays', logParams.lastXDays);
  }
  return this.http.get<Log[]>(this.baseUrl + 'admin/logs', {observe: 'response', params})
  .pipe(
    map(response => {
      paginatedResult.result = response.body;
      if(response.headers.get('Pagination') != null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    })
  );
}
updateUserRoles(user: User, roles: {}) {
  return this.http.post(this.baseUrl + 'admin/editRoles/' + user.userName, roles);
}
clearLogs() {
  return this.http.post(this.baseUrl + 'admin/clearlogs/', {});
}
}
