import { Component, OnInit } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { SignalRService } from 'src/app/_services/signalR.service';
import { AuthService } from 'src/app/_services/auth.service';
import { RoomServiceService } from 'src/app/_services/room-service.service';
import { Room } from 'src/app/_models/room';
import { Refreshable } from 'src/app/_interfaces/refreshable';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';
import { ChatService } from 'src/app/_services/chat.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-chat-window',
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.css']
})
export class ChatWindowComponent implements OnInit {
  userId: number;
  messages: Message[] = [];
  currentRoom: Room;
  pagination: Pagination;
  currentPage = 1;
  itemsPerPage = 5;
  newMessage: string;
  constructor(private signalRService: SignalRService, private route: ActivatedRoute,
              private chatService: ChatService, private alertify: AlertifyService,
              private authService: AuthService, public roomService: RoomServiceService) {
    this.userId = this.authService.decodedToken.nameid;
    this.roomService.roomChanged.subscribe((room: Room) => {this.currentRoom = room; this.loadMessages(); });
    this.signalRService.messageReceived.subscribe((message: Message) => {
      console.log(message);
      this.messages.push(message);
    });
    //this.route.data.subscribe(data => {this.messages = data.messages; });
   }
  ngOnInit() {
  }
  sendMessage() {
    this.signalRService.send(this.roomService.currentRoom.name, this.newMessage);
    this.newMessage = '';
  }
  loadMessages() {
    this.chatService.getRoomHistory(this.currentRoom.name, this.currentPage, this.itemsPerPage)
    .subscribe((res: PaginatedResult<Message[]>) => {this.messages = res.result; this.pagination = res.pagination; },
    error => this.alertify.error(error));
  }
  loadMessagesIncremental() {
    this.chatService.getRoomHistory(this.currentRoom.name, this.pagination.currentPage, this.pagination.itemsPerPage)
    .subscribe((res: PaginatedResult<Message[]>) => {
      this.messages.unshift(...res.result); this.pagination = res.pagination; },
    error => this.alertify.error(error));
  }
  onScrollUp(): void {
    console.log('total pages = ' + this.pagination.totalPages);
    console.log('current page = ' + this.pagination.currentPage);
    if (this.pagination.totalPages > this.pagination.currentPage) {
    this.pagination.currentPage = this.pagination.currentPage + 1;
    this.loadMessagesIncremental();
    }
  }
}
