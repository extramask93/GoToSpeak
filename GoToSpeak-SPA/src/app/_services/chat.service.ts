import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';
import { PaginatedResult } from '../_models/pagination';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

const httpOptions = {
  headers: new HttpHeaders({
    'Authorization': 'Bearer ' + localStorage.getItem('token')
  })
};

@Injectable({
  providedIn: 'root'
})
export class ChatService {
baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }
getUsers(): Observable<User[]> {
  return this.http.get<User[]>(this.baseUrl + 'users', httpOptions);
}
getUser(id): Observable<User>
{
  return this.http.get<User>(this.baseUrl + 'users/' + id, httpOptions);
}
getMessages(id: number, page?, itemsPerPage?, messageContainer?) {
  const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();
  let params = new HttpParams();
  params = params.append('MessageContainer', messageContainer);
  if(page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }
  return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {observe: 'response', params})
  .pipe(
    map(response => {
      paginatedResult.result = response.body;
      if (response.headers.get('Pagination') !== null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    })
  );
}

}
