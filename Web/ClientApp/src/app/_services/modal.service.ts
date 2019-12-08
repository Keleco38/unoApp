import { PickAnyCardComponent } from './../_components/_modals/pick-any-card/pick-any-card.component';
import { KickBanPlayerComponent } from './../_components/_modals/kick-ban-player/kick-ban-player.component';
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
import { User } from '../_models/user';
import { InputPasswordComponent } from '../_components/_modals/input-password/input-password.component';
import { KeyValue } from '@angular/common';
import { Card } from '../_models/card';

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  private _readyPhaseModalPlayers: any;
  private _readyPhaseModalSpectators: any;

  constructor(private _modalService: NgbModal) {}

  hideReadyPhaseDialogs() {
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

  displayInputPasswordModal(
    isTournament: boolean,
    id: string,
    name: string,
    started: boolean,
    numberOfPlayers: number,
    maxNumberOfPlayers: number
  ) {
    var inputPasswordModal = this._modalService.open(InputPasswordComponent, { backdrop: 'static', keyboard: false });
    inputPasswordModal.componentInstance.id = id;
    inputPasswordModal.componentInstance.isTournament = isTournament;
    inputPasswordModal.componentInstance.name = name;
    inputPasswordModal.componentInstance.started = started;
    inputPasswordModal.componentInstance.numberOfPlayers = numberOfPlayers;
    inputPasswordModal.componentInstance.maxNumberOfPlayers = maxNumberOfPlayers;
    return inputPasswordModal;
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
    var userSettingsModal = this._modalService.open(UserSettingsComponent, { backdrop: 'static', keyboard: false });
    userSettingsModal.componentInstance.displayRenameModal.subscribe($e => {
      this.displayRenameModal();
    });
    return userSettingsModal;
  }

  displayContactFormModal() {
    return this._modalService.open(ContactFormComponent, { backdrop: 'static', keyboard: false });
  }

  displayKickBanPlayerModal(isTournament: boolean, userToKick: User) {
    var kickBanModal = this._modalService.open(KickBanPlayerComponent, { backdrop: 'static', keyboard: false });
    kickBanModal.componentInstance.isTournament = isTournament;
    kickBanModal.componentInstance.userToKick = userToKick;
    return kickBanModal;
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

  displayShowCardsModal(cardsAndNames: KeyValue<string, Card[]>[], detailed: boolean) {
    const modalRef = this._modalService.open(ShowCardsComponent, { backdrop: 'static', keyboard: false });
    modalRef.componentInstance.cardsAndNames = cardsAndNames;
    modalRef.componentInstance.detailed = detailed;
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

  displayPickAnyCardModal() {
    return this._modalService.open(PickAnyCardComponent);
  }

  displayPickNumbersToDiscardModal() {
    return this._modalService.open(PickNumbersToDiscardComponent);
  }

  displayPickPromiseKeeperCardModal() {
    return this._modalService.open(PickPromiseCardComponent);
  }
}
