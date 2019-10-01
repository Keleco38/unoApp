/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { ModalObservablesService } from './modal-observables.service';

describe('Service: ModalObservables', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ModalObservablesService]
    });
  });

  it('should ...', inject([ModalObservablesService], (service: ModalObservablesService) => {
    expect(service).toBeTruthy();
  }));
});
