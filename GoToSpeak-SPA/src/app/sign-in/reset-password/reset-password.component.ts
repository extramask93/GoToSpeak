import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  model: any = {};
  constructor(private authService: AuthService, private route: ActivatedRoute, private alertifyService: AlertifyService) {
    route.queryParams.subscribe(params => {
      this.model.token = params.token;
    });
   }

  ngOnInit() {

  }
  send() {
    this.authService.resetPassword(this.model).subscribe(
      response => this.alertifyService.success('Password has been changed'),
      error => this.alertifyService.error(error)
    );
  }

}
