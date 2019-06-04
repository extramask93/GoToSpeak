import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode =  true;
  users: any;
  constructor( private http: HttpClient) { }

  ngOnInit() {
    this.getUsers();
  }
  registerToggle() {
    this.registerMode = true;
  }
  getUsers() {
    this.http.get('http://localhost:5000/api/values').subscribe(response => {
      console.log(response);
      this.users = response;
    },
    error => {
      console.log(error);
    }
     );
  }
  cancelRegisterMode(registeMode: boolean) {
    console.log('event receieved');
    this.registerMode = registeMode;
  }

}
