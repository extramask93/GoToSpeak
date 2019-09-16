import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  bsModalRef: BsModalRef;
  @Input()
  pagination: Pagination;
  @Input()
  users: User[];

  constructor(private adminService: AdminService, private modalService: BsModalService, private alertifyService: AlertifyService) { }

  ngOnInit() {
    this.getUsersWithRoles();
  }
  pageChanged(event: any): void {
    console.log(event);
    this.pagination.currentPage = event.page;
    this.getUsersWithRoles();
  }
  getUsersWithRoles() {
    this.adminService.getUsersWithRoles(this.pagination.currentPage, this.pagination.itemsPerPage)
    .subscribe((res: PaginatedResult<User[]>) => {this.users = res.result; this.pagination = res.pagination;},
    error => this.alertifyService.error(error));
  }
  editRolesModal(user: User) {
    const initialState = {
      user,
      roles: this.getRolesArray(user)
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
    this.bsModalRef.content.updateSelectedRoles.subscribe((values) => {
      const rolesToUpdate = {
        roleNames: [...values.filter(el => el.checked === true).map(el => el.name)]
      };
      if (rolesToUpdate) {
        this.adminService.updateUserRoles(user, rolesToUpdate).subscribe(() => {
          user.roles = [...rolesToUpdate.roleNames];
        }, error => {
          console.log(error);
        });
      }
    });
  }
  private getRolesArray(user: User) {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] = [
      {name: 'User', value: 'User'},
      {name: 'Admin', value: 'Admin'},
      {name: 'Moderator', value: 'Moderator'},
    ];

    for (let i = 0; i < availableRoles.length; i++) {
      let isMatch = false;
      for (let j = 0; j < userRoles.length; j++) {
        if (availableRoles[i].name === userRoles[j]) {
          isMatch = true;
          availableRoles[i].checked = true;
          roles.push(availableRoles[i]);
          break;
        }
      }
      if (!isMatch) {
        availableRoles[i].checked = false;
        roles.push(availableRoles[i]);
      }
    }
    return roles;
  }
}
