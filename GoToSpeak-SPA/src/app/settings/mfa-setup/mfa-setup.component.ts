import { Component, OnInit } from '@angular/core';
import { AccountManagementService } from 'src/app/_services/account-management.service';
import { MfaState } from 'src/app/_models/mfaState';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-mfa-setup',
  templateUrl: './mfa-setup.component.html',
  styleUrls: ['./mfa-setup.component.css']
})
export class MfaSetupComponent implements OnInit {
  mfaState: any = {};
  codes: any = {};
  constructor(private mfaService: AccountManagementService, private alrtify: AlertifyService) { }

  ngOnInit() {
    this.refreshState();
    if (true === this.mfaState.is2faEnabled) {
     this.mfaService.getQrCode().subscribe((model) => this.codes = model, (error) => 
     this.alrtify.error(error));
    }
  }
  refreshState() {
    this.mfaService.getMfaState().subscribe((model) => {console.log(model); this.mfaState = model; },
     error => this.alrtify.error(error));
  }
  showAuthenticator() {
    this.mfaService.getQrCode().subscribe((model) => this.codes = model,
    error => this.alrtify.error(error));
  }
  validateCode() {
    if (false === this.mfaState.is2faEnabled) {
    this.mfaService.enableAuth(this.codes).subscribe(() => this.refreshState(), error => this.alrtify.error(error));
    } else {
      this.mfaService.disableMfa().subscribe((x) => {this.alrtify.success(x['message']); this.refreshState(); }, error => this.alrtify.error(error));
    }
  }
}
