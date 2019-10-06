/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { PlayCardService } from './play-card.service';

describe('Service: PlayCard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PlayCardService]
    });
  });

  it('should ...', inject([PlayCardService], (service: PlayCardService) => {
    expect(service).toBeTruthy();
  }));
});
