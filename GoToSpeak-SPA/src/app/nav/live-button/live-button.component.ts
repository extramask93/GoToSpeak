import { Component, OnInit } from '@angular/core';
import { SignalRService } from 'src/app/_services/signalR.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-live-button',
  templateUrl: './live-button.component.html',
  styleUrls: ['./live-button.component.css']
})
export class LiveButtonComponent implements OnInit {
  on = false;

  constructor(private signalr: SignalRService, alertify: AlertifyService) {
    signalr.connectionEstablished.subscribe(result => {
      this.on = result;
    }, error => alertify.error(error));
   }

  ngOnInit() {
  }

}
