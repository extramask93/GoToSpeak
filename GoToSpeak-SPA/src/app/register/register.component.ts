import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  constructor(private authSerive: AuthService) { }

  ngOnInit() {
  }
  register() {
    console.log(this.model);
    this.authSerive.register(this.model).subscribe(() => {console.log('registaration succedeed'); } , error => {console.log(error); } );
  }
  cancel() {
    this.cancelRegister.emit(false);
    console.log('cnaceled');
  }
}
