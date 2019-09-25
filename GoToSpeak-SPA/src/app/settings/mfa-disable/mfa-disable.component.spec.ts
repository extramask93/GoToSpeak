/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MfaDisableComponent } from './mfa-disable.component';

describe('MfaDisableComponent', () => {
  let component: MfaDisableComponent;
  let fixture: ComponentFixture<MfaDisableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MfaDisableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MfaDisableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
