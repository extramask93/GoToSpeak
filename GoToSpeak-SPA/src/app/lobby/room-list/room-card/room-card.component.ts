import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { Room } from 'src/app/_models/room';

@Component({
  selector: 'app-room-card',
  templateUrl: './room-card.component.html',
  styleUrls: ['./room-card.component.css']
})
export class RoomCardComponent implements OnInit {
  @Input() room: Room;
  @Output() roomSelectedEvent = new EventEmitter<Room>();
  @Output() roomDeletedEvent = new EventEmitter<Room>();
  constructor() { }

  ngOnInit() {
  }

}
