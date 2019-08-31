import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { Log } from 'src/app/_models/log';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-log-viewer',
  templateUrl: './log-viewer.component.html',
  styleUrls: ['./log-viewer.component.css']
})
export class LogViewerComponent implements OnInit {
  logs: Log[];
  constructor(private adminService: AdminService) { }

  ngOnInit() {
    const  params = new  HttpParams().set('level', '1');
    this.adminService.getLogs(params).subscribe((logs: Log[]) => {
      console.log(logs);
      this.logs = logs;
    }, error => {console.log(error); });
  }

}
