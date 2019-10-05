/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { ModalSubscribingService } from './modal-subscribing.service';

describe('Service: ModalSubscribing', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ModalSubscribingService]
    });
  });

  it('should ...', inject([ModalSubscribingService], (service: ModalSubscribingService) => {
    expect(service).toBeTruthy();
  }));
});
