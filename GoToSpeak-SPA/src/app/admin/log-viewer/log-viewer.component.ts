import { Component, OnInit, Input } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { Log } from 'src/app/_models/log';
import { HttpParams } from '@angular/common/http';
import { max } from 'rxjs/operators';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-log-viewer',
  templateUrl: './log-viewer.component.html',
  styleUrls: ['./log-viewer.component.css']
})
export class LogViewerComponent implements OnInit {
  
  minDate = new Date();
  bsRangeValue: Date[];
  maxDate = new Date();
  nameFilter = '';
  level = '1';
  @Input()
  logs: Log[];
  @Input()
  pagination: Pagination;
  constructor(private adminService: AdminService, private alertifyService: AlertifyService) {
    this.maxDate.setDate(this.maxDate.getDate() + 7);
    this.bsRangeValue = [this.minDate, this.maxDate];
   }

  ngOnInit() {
  }
  applyFilters() {

  }
  pageChanged(event: any): void {
    console.log(event);
    this.pagination.currentPage = event.page;
    this.loadLogs();
  }
  loadLogs(): void {
    this.adminService.getLogs(this.pagination.currentPage, this.pagination.itemsPerPage)
    .subscribe((res: PaginatedResult<Log[]>) => {this.logs = res.result; this.pagination = res.pagination;},
    error => this.alertifyService.error(error));
  }

}
