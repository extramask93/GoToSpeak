import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode =  false;
  ngOnInit(): void {
  }
  constructor( private http: HttpClient, private activatedRoute: ActivatedRoute) {
    this.activatedRoute.url.subscribe(url => {
      this.registerMode = false;
 });
  }

  registerToggle() {
    this.registerMode = true;
  }

  cancelRegisterMode(registeMode: boolean) {
    this.registerMode = registeMode;
  }

}
