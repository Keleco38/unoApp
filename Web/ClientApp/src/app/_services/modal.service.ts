import { HubService } from './hub.service';
import { HubService } from 'src/app/_services/hub.service';
import { GameEndedResult } from 'src/app/_models/gameEndedResult';
import { Injectable, OnDestroy } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FirstTimeLaunchComponent } from '../_components/_modals/first-time-launch/first-time-launch.component';
import { GameSetupComponent } from '../_components/_modals/game-setup/game-setup.component';
import { PickBannedCardsComponent } from '../_components/_modals/pick-banned-cards/pick-banned-cards.component';
import { TournamentSetupComponent } from '../_components/_modals/tournament-setup/tournament-setup.component';
import { ContactFormComponent } from '../_components/_modals/contact-form/contact-form.component';
import { UserSettingsComponent } from '../_components/_modals/user-settings/user-settings.component';
import { AdminSectionComponent } from '../_components/_modals/admin-section/admin-section.component';
import { RenameComponent } from '../_components/_modals/rename/rename.component';
import { PickColorComponent } from '../_components/_modals/pick-color/pick-color.component';
import { PickPlayerComponent } from '../_components/_modals/pick-player/pick-player.component';
import { PickDuelNumbersComponent } from '../_components/_modals/pick-duel-numbers/pick-duel-numbers.component';
import { PickCharityCardsComponent } from '../_components/_modals/pick-charity-cards/pick-charity-cards.component';
import { GuessOddEvenNumberComponent } from '../_components/_modals/guess-odd-even-number/guess-odd-even-number.component';
import { DigCardComponent } from '../_components/_modals/dig-card/dig-card.component';
import { BlackjackComponent } from '../_components/_modals/blackjack/blackjack.component';
import { PickNumbersToDiscardComponent } from '../_components/_modals/pick-numbers-to-discard/pick-numbers-to-discard.component';
import { PickPromiseCardComponent } from '../_components/_modals/pick-promise-card/pick-promise-card.component';
import { GameInfoComponent } from '../_components/_modals/game-info/game-info.component';
import { GameEndedResultComponent } from '../_components/_modals/game-ended-result/game-ended-result.component';
import { ConfirmReadyComponent } from '../_components/_modals/confirm-ready/confirm-ready.component';
import { ReadyPhaseSpectatorsComponent } from '../_components/_modals/ready-phase-spectators/ready-phase-spectators.component';
import { ShowCardsComponent } from '../_components/_modals/show-cards/show-cards.component';
import { takeWhile } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ModalService implements OnDestroy {
  private _isAlive = true;
  private _readyPhaseModalPlayers: any;
  private _readyPhaseModalSpectators: any;

  constructor(private _modalService: NgbModal, private _hubService: HubService) {
    this._hubService.updateGameEnded.pipe(takeWhile(() => this._isAlive)).subscribe(gameEndedResult => {
      this.displayGameEndedResultModal(gameEndedResult);
    });
    this._hubService.updateShowReadyPhasePlayers.pipe(takeWhile(() => this._isAlive)).subscribe(isTournament => {
      this.displayReadyPhasePlayersModal(isTournament);
    });
    this._hubService.updateShowReadyPhaseSpectators.pipe(takeWhile(() => this._isAlive)).subscribe(isTournament => {
      this.displayReadyPhaseSpectatorsModal(isTournament);
    });
    this._hubService.updateShowCards.pipe(takeWhile(() => this._isAlive)).subscribe(cardsAndNames => {
      this.displayShowCardsModal(cardsAndNames);
    });
    this._hubService.updateRenameUser.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this.displayRenameModal();
    });
    this._hubService.updateGameStarted.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this.hideReadyPhaseDialogs();
    });
    this._hubService.updateTournamentStarted.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this.hideReadyPhaseDialogs();
    });
  }

  private hideReadyPhaseDialogs() {
    setTimeout(() => {
      if (this._readyPhaseModalPlayers) {
        this._readyPhaseModalPlayers.dismiss();
      }
      if (this._readyPhaseModalSpectators) {
        this._readyPhaseModalSpectators.dismiss();
      }
    }, 500);
  }

  displayRenameModal() {
    return this._modalService.open(RenameComponent, { backdrop: 'static', keyboard: false });
  }

  displayFirstTimeLaunchedModal() {
    return this._modalService.open(FirstTimeLaunchComponent, { backdrop: 'static', keyboard: false });
  }

  displayReadyPhasePlayersModal(isTournament: boolean) {
    this._readyPhaseModalPlayers = this._modalService.open(ConfirmReadyComponent, { backdrop: 'static', keyboard: false });
    this._readyPhaseModalPlayers.componentInstance.isTournament = isTournament;
    return this._readyPhaseModalPlayers;
  }

  displayReadyPhaseSpectatorsModal(isTournament: boolean) {
    this._readyPhaseModalSpectators = this._modalService.open(ReadyPhaseSpectatorsComponent, { backdrop: 'static', keyboard: false });
    this._readyPhaseModalSpectators.componentInstance.isTournament = isTournament;
    return this._readyPhaseModalSpectators;
  }

  displayUserSettingsModal() {
    return this._modalService.open(UserSettingsComponent, { backdrop: 'static', keyboard: false });
  }

  displayContactFormModal() {
    return this._modalService.open(ContactFormComponent, { backdrop: 'static', keyboard: false });
  }

  displayGameInfoModal() {
    return this._modalService.open(GameInfoComponent, { scrollable: true });
  }

  displayGameSetupModal() {
    return this._modalService.open(GameSetupComponent, { backdrop: 'static', keyboard: false });
  }

  displayTournamentSetupModal() {
    return this._modalService.open(TournamentSetupComponent, { backdrop: 'static', keyboard: false });
  }

  displayAdminSectionModal() {
    return this._modalService.open(AdminSectionComponent, { backdrop: 'static', keyboard: false });
  }

  displayPickCardsToBanModal() {
    return this._modalService.open(PickBannedCardsComponent, { backdrop: 'static', keyboard: false });
  }

  displayPickColorModal() {
    return this._modalService.open(PickColorComponent);
  }

  displayShowCardsModal(cardsAndNames) {
    const modalRef = this._modalService.open(ShowCardsComponent, { backdrop: 'static' });
    modalRef.componentInstance.cardsAndNames = cardsAndNames;
    return modalRef;
  }

  displayPickPlayerModal() {
    return this._modalService.open(PickPlayerComponent);
  }

  displayDigCardModal() {
    return this._modalService.open(DigCardComponent);
  }

  displayBlackjackModal() {
    return this._modalService.open(BlackjackComponent, { backdrop: 'static', keyboard: false });
  }

  displayGameEndedResultModal(gameEndedResult: GameEndedResult) {
    var modalRef = this._modalService.open(GameEndedResultComponent, { backdrop: 'static', keyboard: false });
    modalRef.componentInstance.gameEndedResult = gameEndedResult;
    return modalRef;
  }

  displayGuessOdEvenNumbersModal() {
    return this._modalService.open(GuessOddEvenNumberComponent);
  }

  displayPickCharityCardsModal() {
    return this._modalService.open(PickCharityCardsComponent);
  }

  displayPickDuelNumbers() {
    return this._modalService.open(PickDuelNumbersComponent);
  }

  displayPickNumbersToDiscardModal() {
    return this._modalService.open(PickNumbersToDiscardComponent);
  }

  displayPickPromiseKeeperCardModal() {
    return this._modalService.open(PickPromiseCardComponent);
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
