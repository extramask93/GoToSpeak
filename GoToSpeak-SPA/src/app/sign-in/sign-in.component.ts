import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {
  model: any = {};
  info: string;
  constructor(private authService: AuthService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
  }
  login() {
    console.log(this.model);
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('logged in successfully');
    },
    error => {
      if(error.code === 101) {
        this.router.navigate(['/signin2fa']);
        this.alertify.error(error.message);
      }
      else {
        this.alertify.error(error);
        this.info = error.message;
      }
    },
    () => {this.router.navigate(['/members']); }
    );
  }
}
