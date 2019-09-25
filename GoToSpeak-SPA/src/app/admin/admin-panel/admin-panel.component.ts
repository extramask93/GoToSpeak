import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Log } from 'src/app/_models/log';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {
  logs: Log[];
  users: User[];
  paginationLogs: Pagination;
  paginationUsers: Pagination;
  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
     this.logs = data.logs.result;
     this.paginationLogs = data.logs.pagination;
     this.users = data.users.result;
     this.paginationUsers = data.users.pagination;
    });
  }

}
