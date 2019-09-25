import { Injectable, Output, EventEmitter } from '@angular/core';
import { Room } from '../_models/room';
import { SignalRService } from './signalR.service';

@Injectable({
  providedIn: 'root'
})
export class RoomServiceService {
@Output() public roomChanged: EventEmitter<Room> = new EventEmitter();
currentRoom: Room = {id : 9999, name : 'lobby'};
constructor(private signalrService: SignalRService) {
  this.roomChanged.subscribe((room: Room) => this.currentRoom = room);
  this.signalrService.roomDeleted.subscribe((room: Room ) => {
    if (room.name === this.currentRoom.name){
      const roomnew: Room = {id: 9999, name: 'lobby'};
      this.roomChanged.emit(roomnew);
    }
  });
}

}
