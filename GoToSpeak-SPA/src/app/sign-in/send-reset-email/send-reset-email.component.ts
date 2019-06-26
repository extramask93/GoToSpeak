import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-send-reset-email',
  templateUrl: './send-reset-email.component.html',
  styleUrls: ['./send-reset-email.component.css']
})
export class SendResetEmailComponent implements OnInit {
  model: any = {};
  constructor(private authService: AuthService, private alertifyService: AlertifyService) { }

  ngOnInit() {
  }
  send() {
    this.authService.sendEmail(this.model).subscribe(response => this.alertifyService.success('Confirmation email has been sent'),
    error => this.alertifyService.error(error));
  }
}
