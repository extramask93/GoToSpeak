import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }
getUsersWithRoles() {
  return this.http.get(this.baseUrl + 'admin/usersWithRoles');
}
getLogs(params: HttpParams) {
  return this.http.get(this.baseUrl + 'admin/logs', {params});
}
}
