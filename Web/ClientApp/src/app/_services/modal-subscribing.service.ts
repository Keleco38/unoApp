import { ModalService } from './modal.service';
import { Injectable, OnDestroy } from '@angular/core';
import { takeWhile } from 'rxjs/operators';
import { HubService } from './hub.service';

@Injectable({
  providedIn: 'root'
})
export class ModalSubscribingService implements OnDestroy {
  private _isAlive = true;

  constructor(private _hubService: HubService, private _modalService: ModalService) {
    this._hubService.updateGameEnded.pipe(takeWhile(() => this._isAlive)).subscribe(gameEndedResult => {
      this._modalService.displayGameEndedResultModal(gameEndedResult);
    });
    this._hubService.updateShowReadyPhasePlayers.pipe(takeWhile(() => this._isAlive)).subscribe(isTournament => {
      this._modalService.displayReadyPhasePlayersModal(isTournament);
    });
    this._hubService.updateShowReadyPhaseSpectators.pipe(takeWhile(() => this._isAlive)).subscribe(isTournament => {
      this._modalService.displayReadyPhaseSpectatorsModal(isTournament);
    });
    this._hubService.updateShowCards.pipe(takeWhile(() => this._isAlive)).subscribe(cardsAndNames => {
      this._modalService.displayShowCardsModal(cardsAndNames);
    });
    this._hubService.updateRenameUser.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this._modalService.displayRenameModal();
    });
    this._hubService.updateGameStarted.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this._modalService.hideReadyPhaseDialogs();
    });
    this._hubService.updateTournamentStarted.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this._modalService.hideReadyPhaseDialogs();
    });
  }

  something(){
    return null;
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
