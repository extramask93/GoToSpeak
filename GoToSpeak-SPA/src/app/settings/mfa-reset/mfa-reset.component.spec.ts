/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MfaResetComponent } from './mfa-reset.component';

describe('MfaResetComponent', () => {
  let component: MfaResetComponent;
  let fixture: ComponentFixture<MfaResetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MfaResetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MfaResetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
