import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-login-history',
  templateUrl: './login-history.component.html',
  styleUrls: ['./login-history.component.css']
})
export class LoginHistoryComponent implements OnInit {
  user: any = {};
  constructor() { }

  ngOnInit() {
    const userString = localStorage.getItem('user');
    try {
      this.user = JSON.parse(userString);
    } catch (ex) {
      console.error(ex);
    }
  }

}
