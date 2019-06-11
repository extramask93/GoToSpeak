import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../_services/signalR.service';
import { HttpClient } from '@aspnet/signalr';

@Component({
  selector: 'app-temp',
  templateUrl: './temp.component.html',
  styleUrls: ['./temp.component.css']
})
export class TempComponent implements OnInit {

  constructor(public signalRService: SignalRService) { }

  ngOnInit() {
    this.signalRService.startConnection();

  }

}
