import { Component, OnInit } from '@angular/core';
import { User } from '../_models/user';
import { AlertifyService } from '../_services/alertify.service';
import { ChatService } from '../_services/chat.service';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'app-memberList',
  templateUrl: './memberList.component.html',
  styleUrls: ['./memberList.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  constructor(private chatService: ChatService, private alertify: AlertifyService) { }

  ngOnInit() {
  }
  loadUsers() {
    this.chatService.getUsers().subscribe((users: User[]) => {this.users = users; }, error => {this.alertify.error(error); });
  }
}
