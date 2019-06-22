import { Component, OnInit } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { SignalRService } from 'src/app/_services/signalR.service';
import { AuthService } from 'src/app/_services/auth.service';
import { RoomServiceService } from 'src/app/_services/room-service.service';
import { Room } from 'src/app/_models/room';
import { Refreshable } from 'src/app/_interfaces/refreshable';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-chat-window',
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.css']
})
export class ChatWindowComponent implements OnInit {
  userId: number;
  messages: Message[] = [];
  newMessage: string;
  constructor(private signalRService: SignalRService, private route: ActivatedRoute,
              private authService: AuthService, public roomService: RoomServiceService) {
    this.userId = this.authService.decodedToken.nameid;
    this.roomService.roomChanged.subscribe((room: Room) => { this.signalRService.getMessageHistory(room.name)
      .then((messages: Message[]) => this.messages = messages ); });
    this.signalRService.messageReceived.subscribe((message: Message) => {
      console.log(message);
      this.messages.push(message);
    });
    this.route.data.subscribe(data => {this.messages = data.messages; });
   }
  ngOnInit() {
  }
  sendMessage() {
    this.signalRService.send(this.roomService.currentRoom.name, this.newMessage);
    this.newMessage = '';
  }
}
