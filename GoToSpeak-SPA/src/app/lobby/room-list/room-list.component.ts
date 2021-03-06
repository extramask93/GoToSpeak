import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { SignalRService } from 'src/app/_services/signalR.service';
import { Room } from 'src/app/_models/room';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { RoomServiceService } from 'src/app/_services/room-service.service';
import { Refreshable } from 'src/app/_interfaces/refreshable';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';
import { ChatService } from 'src/app/_services/chat.service';

@Component({
  selector: 'app-room-list',
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.css']
})
export class RoomListComponent implements OnInit {
  rooms: Room[] = [];
  users: User[] = [];
  paginationRooms: Pagination;
  @Output() roomChanged = new EventEmitter<Room>();
  newRoomName: string;
  constructor(private signalr: SignalRService,
              private chatService: ChatService,
              private route: ActivatedRoute, private alertifyService: AlertifyService, private roomService: RoomServiceService) {
    this.signalr.roomAdded.subscribe((room: Room) => {this.rooms.push(room); }, error => alertifyService.error(error));
    this.signalr.roomDeleted.subscribe((room: Room) => { console.log(room); this.rooms.forEach( (item, index) => {
      if (item.name === room.name) {this.rooms.splice(index, 1); }
    }); }, error => this.alertifyService.error(error) );
    this.route.data.subscribe(data => {this.rooms = data.rooms.result;
                                       this.paginationRooms = data.rooms.pagination; });
    // this.signalr.connectionEstablished.subscribe(() => {this.signalr.getRooms().then((rooms: Room[]) => this.rooms = rooms); });
   }
  ngOnInit() {
  }
  pageChanged(event: any): void {
    console.log(event);
    this.paginationRooms.currentPage = event.page;
    this.chatService
    .getRooms(this.paginationRooms.currentPage, this.paginationRooms.itemsPerPage)
    .subscribe((res: PaginatedResult<Room[]>) => {
      this.rooms = res.result; this.paginationRooms = res.pagination; },
    error => this.alertifyService.error(error));
  }
  joinRoom(room: Room) {
    this.signalr.joinRoom(room.name).then(() =>
     { console.log('from room list' + room.name); this.roomService.roomChanged.emit(room)}, error => console.log(error));
  }
  retriveUsers(name: string) {
    this.signalr.getUsers(name).then((users: User[]) => { this.users = users; console.log(users); } );
  }
  deleteRoom(room: Room) {
    console.log('asdad');
    this.signalr.removeRoom(room.name);
  }
  createRoom() {
    this.signalr.createRoom(this.newRoomName);
    this.newRoomName = '';
  }
}
