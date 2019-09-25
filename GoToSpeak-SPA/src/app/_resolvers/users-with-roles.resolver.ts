import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AdminService } from '../_services/admin.service';
import { User } from '../_models/user';



@Injectable()
export class UsersWithRolesResolver implements Resolve<User[]> {
    users: User[];
    pageNumber = 1;
    pageSize = 5;
    constructor(private router: Router, private alertify: AlertifyService, private adminService: AdminService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.adminService.getUsersWithRoles(this.pageNumber, this.pageSize).pipe(catchError(() => {
            this.alertify.error('Problem retrieving data');
            this.router.navigate(['/home']);
            return of(null);
        }));
    }
}
