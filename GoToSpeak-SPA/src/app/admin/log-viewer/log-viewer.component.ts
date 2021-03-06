import { Component, OnInit, Input } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { Log } from 'src/app/_models/log';
import { HttpParams } from '@angular/common/http';
import { max } from 'rxjs/operators';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UtcPipePipe } from 'src/app/_pipes/utcPipe.pipe';

@Component({
  selector: 'app-log-viewer',
  templateUrl: './log-viewer.component.html',
  styleUrls: ['./log-viewer.component.css']
})
export class LogViewerComponent implements OnInit {
  levelList = [{value: 1, display: 'Level 1'}, {value: 2, display: 'Level 2'}, {value: 3, display: 'Level 3'}];
  logParams: any = {};
  @Input()
  logs: Log[];
  @Input()
  pagination: Pagination;
  constructor(private adminService: AdminService, private alertifyService: AlertifyService) {
   }

  ngOnInit() {
    this.resetFilters();
  }
  resetFilters() {
    this.logParams.lastXDays = 0;
    this.logParams.userName = '';
    this.logParams.level = 0;
    this.loadLogs();
  }
  pageChanged(event: any): void {
    console.log(event);
    this.pagination.currentPage = event.page;
    this.loadLogs();
  }
  loadLogs(): void {
    console.log(this.logParams);
    this.adminService.getLogs(this.pagination.currentPage, this.pagination.itemsPerPage, this.logParams)
    .subscribe((res: PaginatedResult<Log[]>) => {this.logs = res.result; this.pagination = res.pagination; },
    error => this.alertifyService.error(error));
  }
  clearLogs(): void {
    this.adminService.clearLogs().subscribe(() => {
      this.loadLogs();
    }, error => {
      console.log(error);
    });
  }


}
