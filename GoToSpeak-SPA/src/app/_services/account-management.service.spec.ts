/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { AccountManagementService } from './account-management.service';

describe('Service: AccountManagement', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AccountManagementService]
    });
  });

  it('should ...', inject([AccountManagementService], (service: AccountManagementService) => {
    expect(service).toBeTruthy();
  }));
});
