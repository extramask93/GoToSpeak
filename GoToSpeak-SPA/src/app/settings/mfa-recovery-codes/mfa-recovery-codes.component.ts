import { Component, OnInit } from '@angular/core';
import { AccountManagementService } from 'src/app/_services/account-management.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-mfa-recovery-codes',
  templateUrl: './mfa-recovery-codes.component.html',
  styleUrls: ['./mfa-recovery-codes.component.css']
})
export class MfaRecoveryCodesComponent implements OnInit {
  codes: string[];
  constructor(private accountManagement: AccountManagementService, private alrtify: AlertifyService) { }

  ngOnInit() {
  }
  getRecoveryCodes() {
    this.accountManagement.generateRecoveryCodes().subscribe(value => this.codes = value.codes, error => this.alrtify.error(error));
  }
}
