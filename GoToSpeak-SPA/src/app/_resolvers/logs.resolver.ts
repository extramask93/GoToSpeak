import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Log } from '../_models/log';
import { AdminService } from '../_services/admin.service';



@Injectable()
export class LogsResolver implements Resolve<Log[]> {
    logs: Log[];
    pageNumber = 1;
    pageSize = 5;
    constructor(private router: Router, private alertify: AlertifyService, private adminService: AdminService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Log[]> {
        return this.adminService.getLogs(this.pageNumber, this.pageSize).pipe(catchError(() => {
            this.alertify.error('Problem retrieving data');
            this.router.navigate(['/home']);
            return of(null);
        }));
    }
}
