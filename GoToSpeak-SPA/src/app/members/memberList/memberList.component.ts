import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { AlertifyService } from '../../_services/alertify.service';
import { ChatService } from '../../_services/chat.service';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';
import { SignalRService } from 'src/app/_services/signalR.service';
import { Message } from 'src/app/_models/message';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'app-memberList',
  templateUrl: './memberList.component.html',
  styleUrls: ['./memberList.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[] ;
  recipientId: number;
  paginationUsers: Pagination;
  constructor(private chatService: ChatService, private alertify: AlertifyService,
              private route: ActivatedRoute, private authService: AuthService,
              private signalRService: SignalRService) {
                this.signalRService.messageReceived.subscribe((message: Message) => {
                  for (const user of this.users) {
                    if (user.id === message.recipientId) {
                      user.isNewMessage = true;
                    }
                  }
                });
               }

  ngOnInit() {
    this.recipientId  = +this.authService.decodedToken.nameid;
    this.route.data.subscribe(data => {this.users = data.users.result;
                                       this.paginationUsers = data.users.pagination; });
  }

  pageChanged(event: any): void {
    console.log(event);
    this.paginationUsers.currentPage = event.page;
    this.loadUsers();
  }
  loadUsers(): void {
    this.chatService.getUsers(this.paginationUsers.currentPage, this.paginationUsers.itemsPerPage)
    .subscribe((res: PaginatedResult<User[]>) => {this.users = res.result; this.paginationUsers = res.pagination; },
    error => this.alertify.error(error));
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
