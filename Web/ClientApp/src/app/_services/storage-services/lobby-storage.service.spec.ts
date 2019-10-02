/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { LobbyStorageService } from './lobby-storage.service';

describe('Service: LobbyStorage', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LobbyStorageService]
    });
  });

  it('should ...', inject([LobbyStorageService], (service: LobbyStorageService) => {
    expect(service).toBeTruthy();
  }));
});
