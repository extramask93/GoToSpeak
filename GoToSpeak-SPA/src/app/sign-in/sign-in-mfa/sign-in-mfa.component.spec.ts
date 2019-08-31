/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SignInMfaComponent } from './sign-in-mfa.component';

describe('SignInMfaComponent', () => {
  let component: SignInMfaComponent;
  let fixture: ComponentFixture<SignInMfaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SignInMfaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SignInMfaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
