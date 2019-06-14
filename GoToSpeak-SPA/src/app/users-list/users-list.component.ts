import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../_services/signalR.service';
import { Message } from '../_models/message';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.css']
})
export class UsersListComponent implements OnInit {
  messages: Message[] = [];
  newMessage: any = {};
  constructor(private signalRService: SignalRService, private authService: AuthService) {
    this.signalRService.globalMessageReceived.subscribe((message: Message) => {
      console.log(message);
      this.messages.push(message);
    });
   }

  ngOnInit() {
  }
  sendMessage() {
    this.signalRService.sendGlobalMessage(this.newMessage);
    this.newMessage.content = '';
  }
}
