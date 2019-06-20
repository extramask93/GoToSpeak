import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Room } from 'src/app/_models/room';
import { SignalRService } from 'src/app/_services/signalR.service';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.css']
})
export class LobbyComponent implements OnInit {
  public roomChangedEvent: Event;
  constructor(private signalRService: SignalRService) { }

  ngOnInit() {
  }

}
