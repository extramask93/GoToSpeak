import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-password-change',
  templateUrl: './password-change.component.html',
  styleUrls: ['./password-change.component.css']
})
export class PasswordChangeComponent implements OnInit {
  model: any ={};
  constructor(private authService: AuthService, private alertifyService: AlertifyService) { }

  ngOnInit() {
  }
  resetPassword() {
    this.authService.resetPasswordLoggedIn(this.model).subscribe(x => this.alertifyService.success('Password has been changed'),
     error => this.alertifyService.error(error));
  }
}
