/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { TournamentStorageService } from './tournament-storage.service';

describe('Service: TournamentStorage', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TournamentStorageService]
    });
  });

  it('should ...', inject([TournamentStorageService], (service: TournamentStorageService) => {
    expect(service).toBeTruthy();
  }));
});
