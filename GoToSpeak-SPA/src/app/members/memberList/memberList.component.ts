import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { AlertifyService } from '../../_services/alertify.service';
import { ChatService } from '../../_services/chat.service';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';
import { SignalRService } from 'src/app/_services/signalR.service';
import { Message } from 'src/app/_models/message';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'app-memberList',
  templateUrl: './memberList.component.html',
  styleUrls: ['./memberList.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[] ;
  recipientId: number;
  constructor(private chatService: ChatService, private alertify: AlertifyService,
              private route: ActivatedRoute, private authService: AuthService,
              private signalRService: SignalRService) {
                this.signalRService.connectionEstablished.subscribe((b: boolean) => {this.signalRService.loadUsers(); });
                this.signalRService.messageReceived.subscribe((message: Message) => {
                  for (const user of this.users) {
                    if (user.id === message.recipientId) {
                      console.log(user);
                      user.isNewMessage = true;
                    }
                  }
                });
               }

  ngOnInit() {
    this.recipientId  = +this.authService.decodedToken.nameid;
    this.route.data.subscribe(data => {this.users = data.users; });
  }
  handleChange(recipientId: number) {
    this.recipientId = recipientId;
    for (const user of this.users) {
      if (user.id === recipientId) {
        user.isNewMessage = false;
      }
    }
  }
}
