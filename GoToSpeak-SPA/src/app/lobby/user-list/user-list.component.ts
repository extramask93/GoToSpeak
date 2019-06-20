import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { Room } from 'src/app/_models/room';
import { RoomServiceService } from 'src/app/_services/room-service.service';
import { SignalRService } from 'src/app/_services/signalR.service';
import { Refreshable } from 'src/app/_interfaces/refreshable';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit{
  users: User[];
  constructor(private signalrService: SignalRService,
              private route: ActivatedRoute, private roomService: RoomServiceService) {
    this.signalrService.userJoined.subscribe((user: User) => this.users.push(user));
    this.signalrService.userLeft.subscribe((user: User) => {
      this.users = this.users.filter(item => item.id !== user.id );
    });
    roomService.roomChanged.subscribe((room: Room) =>
     { console.log('log from user list' + room.name);
       this.signalrService.getUsers(room.name).then((users: User[]) => this.users = users); });
    this.route.data.subscribe(data => {this.users = data.users; });

    }

  ngOnInit() {

  }
  refresh() {
    this.signalrService.getUsers(this.roomService.currentRoom.name).then((users: User[]) => this.users = users);
  }
}
