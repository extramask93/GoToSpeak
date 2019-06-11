import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { AlertifyService } from '../../_services/alertify.service';
import { ChatService } from '../../_services/chat.service';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'app-memberList',
  templateUrl: './memberList.component.html',
  styleUrls: ['./memberList.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  recipientId: number;
  constructor(private chatService: ChatService, private alertify: AlertifyService,
              private route: ActivatedRoute, private authService: AuthService) { }

  ngOnInit() {
    this.recipientId  = +this.authService.decodedToken.nameid;
    this.route.data.subscribe(data => {this.users = data.users;});
  }
  loadUsers() {
    this.chatService.getUsers().subscribe((users: User[]) =>
     {this.users = users; console.log(users); }, error => {this.alertify.error(error); });
  }
  handleChange(recipientId: number) {
    this.recipientId = recipientId;
  }
}
