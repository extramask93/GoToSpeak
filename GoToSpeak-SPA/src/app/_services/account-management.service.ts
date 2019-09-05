import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from 'src/environments/environment';
import { MfaState } from '../_models/mfaState';
import { MfaCodes } from '../_models/mfaCodes';
import { Observable} from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { CodesArray } from '../_models/codesArray';

@Injectable({
  providedIn: 'root'
})
export class AccountManagementService {
baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }


getMfaState(): Observable<MfaState> {
  console.log('dupa');
  return this.http.get<MfaState>(this.baseUrl + 'manage/twofactorauth');
}
disableMfa() {
 return this.http.post(this.baseUrl + 'manage/disable2fa', null);
}
getQrCode() {
  return this.http.get<MfaCodes>(this.baseUrl + 'manage/enableauth');
}
enableAuth(model: MfaCodes) {
  return this.http.post(this.baseUrl + 'manage/enableauth', model);
}
generateRecoveryCodes(): Observable<CodesArray> {
  return this.http.post<CodesArray>(this.baseUrl + 'manage/generatecodes', null);
}
}
