/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { GameStorageService } from './game-storage.service';

describe('Service: GameStorage', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [GameStorageService]
    });
  });

  it('should ...', inject([GameStorageService], (service: GameStorageService) => {
    expect(service).toBeTruthy();
  }));
});
