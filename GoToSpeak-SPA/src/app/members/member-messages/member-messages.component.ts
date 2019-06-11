import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { ChatService } from 'src/app/_services/chat.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { tap } from 'rxjs/operators';
import { SignalRService } from 'src/app/_services/signalR.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  connected = false;
  _recipientId: number;
  userId: number;
  newMessage: any = {};
  @Input()
  public set recipientId(val: number) {
    this._recipientId = val;
    if (this.connected) {
      this.signalRService.loadHistory(val);
    }
  }
  messages: Message[];
  user: User;
  constructor(private chatService: ChatService, private authSerive: AuthService, private alertify: AlertifyService,
              private signalRService: SignalRService) {
                this.userId = this.authSerive.decodedToken.nameid;
                this.signalRService.connectionEstablished.subscribe((b: boolean) => {
                  this.connected = b; this.signalRService.loadHistory(this.userId); });
                this.signalRService.historyReceived.subscribe(messages => {
                  console.log(messages);
                  this.messages = messages;
                }, error => {
                  this.alertify.error(error);
                });
                this.signalRService.messageReceived.subscribe((message: Message) => {
                  this.messages.push(message);
                });
               }
  ngOnInit() {}
  sendMessage() {
    this.newMessage.recipientId = this._recipientId;
    this.signalRService.sendMessage(this.newMessage);
    console.log(this.newMessage);
    this.newMessage.content = '';
  }
}
