import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { ChatService } from 'src/app/_services/chat.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { tap } from 'rxjs/operators';
import { SignalRService } from 'src/app/_services/signalR.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  connected = false;
  // tslint:disable-next-line:variable-name
  _recipientId: number;
  userId: number;
  newMessage: any = {};
  @Input()
  public set recipientId(val: number) {
    this._recipientId = val;
    this.loadMessages();
  }
  messages: Message[];
  user: User;
  constructor(private chatService: ChatService, private authSerive: AuthService, private alertify: AlertifyService,
              private signalRService: SignalRService, private route: ActivatedRoute) {
                this.userId = this.authSerive.decodedToken.nameid;
                this.signalRService.messageReceived.subscribe((message: Message) => {
                  this.messages.push(message);
                });
               }
  ngOnInit() {
  }
  loadMessages() {
    const currentUserId = +this.authSerive.decodedToken.nameid;
    this.chatService.getMessageThread(this.authSerive.decodedToken.nameid, this._recipientId)
    .pipe(
      tap(messages => {
        // tslint:disable-next-line:prefer-for-of
        for (let i = 0; i < messages.length; i++) {
          if (messages[i].isRead === false && messages[i].recipientId === currentUserId) {
            // this.chatService.markAsRead(currentUserId, messages[i].id);
          }
        }
      })
    )
    .subscribe(messages => {
      this.messages = messages;
    }, error => {
      this.alertify.error(error);
    });
  }
  sendMessage() {
    this.newMessage.recipientId = this._recipientId;
    this.signalRService.sendMessage(this.newMessage);
    console.log(this.newMessage);
    this.newMessage.content = '';
  }
}
