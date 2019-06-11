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
                //this.signalRService.userJoined.subscribe((user: User) => {console.log(user); this.users.push(user); });
                //this.signalRService.userLeft.subscribe((user: User) =>{
                  //this.users.forEach((item, index) => {if(item.id === user.id){this.users.splice(index,1);}})} )
                this.signalRService.usersReceived.subscribe((users: User[]) => {
                   this.users = users; console.log(this.users); }, error => {this.alertify.error(error); });
                this.signalRService.messageReceived.subscribe((message: Message) => {
                  for(var user of this.users) {
                    if (user.id === message.recipientId) {
                      console.log(user);
                      user.isNewMessage = true;
                    }
                  }
                });
               }

  ngOnInit() {
    this.recipientId  = +this.authService.decodedToken.nameid;
  }
  handleChange(recipientId: number) {
    this.recipientId = recipientId;
    for(var user of this.users) {
      if(user.id === recipientId) {
        user.isNewMessage = false;
      }
    }
  }
}
