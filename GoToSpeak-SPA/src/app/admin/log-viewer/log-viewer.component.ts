import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { Log } from 'src/app/_models/log';
import { HttpParams } from '@angular/common/http';
import { max } from 'rxjs/operators';

@Component({
  selector: 'app-log-viewer',
  templateUrl: './log-viewer.component.html',
  styleUrls: ['./log-viewer.component.css']
})
export class LogViewerComponent implements OnInit {
  logs: Log[];
  minDate = new Date();
  bsRangeValue: Date[];
  maxDate = new Date();
  nameFilter = '';
  level = '1';

  constructor(private adminService: AdminService) {
    this.maxDate.setDate(this.maxDate.getDate() + 7);
    this.bsRangeValue = [this.minDate, this.maxDate];
   }

  ngOnInit() {
    const  params = new  HttpParams();
    this.adminService.getLogs(params).subscribe((logs: Log[]) => {
      console.log(logs);
      this.logs = logs;
    }, error => {console.log(error); });
  }
  applyFilters() {
    const params  = new HttpParams().set('level', this.level).set('name', this.nameFilter)
    .set('from', this.minDate.toDateString()).set('to', this.maxDate.toISOString());
    this.adminService.getLogs(params).subscribe((logs: Log[]) => {
      console.log(logs);
      this.logs = logs;
    }, error => {console.log(error); });
  }

}
