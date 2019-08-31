import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-in-mfa',
  templateUrl: './sign-in-mfa.component.html',
  styleUrls: ['./sign-in-mfa.component.css']
})
export class SignInMfaComponent implements OnInit {
  model: any = {};
  info: string;
  constructor(private authService: AuthService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
  }
  login() {
    console.log(this.model);
    this.authService.login2fa(this.model).subscribe(next => {
      this.alertify.success('logged in successfully');
    },
    error => {
      this.alertify.error(error);
      this.info = error;
    },
    () => {this.router.navigate(['/members']); }
    );
  }

}
