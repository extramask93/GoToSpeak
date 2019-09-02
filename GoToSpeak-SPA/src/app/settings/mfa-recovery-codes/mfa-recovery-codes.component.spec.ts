/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MfaRecoveryCodesComponent } from './mfa-recovery-codes.component';

describe('MfaRecoveryCodesComponent', () => {
  let component: MfaRecoveryCodesComponent;
  let fixture: ComponentFixture<MfaRecoveryCodesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MfaRecoveryCodesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MfaRecoveryCodesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
