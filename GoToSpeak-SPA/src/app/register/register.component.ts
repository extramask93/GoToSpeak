import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  constructor(private authSerive: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }
  register() {
    console.log(this.model);
    this.authSerive.register(this.model).subscribe(() => {
      this.alertify.success('registaration succedeed'); } , error => {this.alertify.error(error); }
      );
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
