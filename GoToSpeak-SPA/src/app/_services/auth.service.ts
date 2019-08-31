import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams} from '@angular/common/http';
import {map, catchError} from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import {BehaviorSubject} from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl;
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

constructor(private http: HttpClient) { }

changeMemberPhoto(url: string) {
  this.photoUrl.next(url);
}

login(model: any) {
  return this.http.post(this.baseUrl + 'auth/login', model)
    .pipe(
    map((response: any) => {
      const user = response;
      if (user) {
        localStorage.setItem('token', user.token);
        localStorage.setItem('user', JSON.stringify(user.user));
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        this.currentUser = user.user;
        this.changeMemberPhoto(this.currentUser.photoUrl);
      }
    })
  );
}
login2fa(model: any) {
  return this.http.post(this.baseUrl + 'auth/login2fa', model)
    .pipe(
    map((response: any) => {
      const user = response;
      if (user) {
        localStorage.setItem('token', user.token);
        localStorage.setItem('user', JSON.stringify(user.user));
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        this.currentUser = user.user;
        this.changeMemberPhoto(this.currentUser.photoUrl);
      }
    })
  );
}
register(model: any) {
  return this.http.post(this.baseUrl + 'auth/register', model);
}
loggedIn() {
  const token = localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired(token);
}
sendEmail(model: any) {
  return this.http.post(this.baseUrl + 'auth/emailReset', model);
}
resetPassword(model: any) {
  return this.http.post(this.baseUrl + 'auth/reset', model);
}
roleMatch(allowedRoles): boolean {
  let isMatch = false;
  const userRoles = this.decodedToken.role as Array<string>;
  allowedRoles.forEach(element => {
    if (userRoles.includes(element)) {
      isMatch = true;
      return;
    }
  });
  return isMatch;
}
}
