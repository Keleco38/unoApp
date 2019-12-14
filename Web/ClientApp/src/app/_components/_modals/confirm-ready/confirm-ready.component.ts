import { UtilityService } from "./../../../_services/utility.service";
import { CardValue } from "./../../../_models/enums";
import { Card } from "./../../../_models/card";
import { TournamentStorageService } from "./../../../_services/storage-services/tournament-storage.service";
import { GameStorageService } from "./../../../_services/storage-services/game-storage.service";
import { ToastrService } from "ngx-toastr";
import { HubService } from "./../../../_services/hub.service";
import { Component, OnInit, Input, OnDestroy } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { takeWhile } from "rxjs/operators";

@Component({
  selector: "app-confirm-ready",
  templateUrl: "./confirm-ready.component.html",
  styleUrls: ["./confirm-ready.component.css"]
})
export class ConfirmReadyComponent implements OnInit, OnDestroy {
  @Input("isTournament") isTournament: boolean;

  private _isAlive = true;
  private _interval;
  private _countDown = 10000;

  timer: number = 10;
  isReady: boolean = false;
  readyPlayersLeft: string[];
  originallyTotalPlayersCount: number = 0;
  bannedCards: CardValue[];
  roundsToWin: number;
  numberOfStandardDecks: number;
  matchingCardStealsTurn: boolean;
  reverseShouldSkipTurnInTwoPlayers: boolean;
  drawAutoPlay: boolean;
  spectatorsCanViewHands: boolean;
  wildCardCanBePlayedOnlyIfNoOtherOptions: boolean;
  limitColorChangingCards: boolean;

  constructor(
    private _activeModal: NgbActiveModal,
    private _toastrService: ToastrService,
    private _hubService: HubService,
    private _gameStorageService: GameStorageService,
    private _tournamentStorageService: TournamentStorageService,
    private _utilityService: UtilityService
  ) { }

  ngOnInit() {
    if (this.isTournament) {
      this._countDown = 20000;
      this.timer = 20;
    }
    this._interval = setInterval(() => {
      this._countDown -= 1000;
      this.timer = Math.floor(this._countDown / 1000);
      if (this.timer <= 0) {
        this._toastrService.error(
          `Players not ready: ${this.readyPlayersLeft.join(", ")}`
        );
        this._activeModal.dismiss();
      }
    }, 1000);
    if (!this.isTournament) {
      this._gameStorageService.activeGame
        .pipe(takeWhile(() => this._isAlive))
        .subscribe(game => {
          this.readyPlayersLeft = game.readyPlayersLeft;
          if (this.originallyTotalPlayersCount == 0) {
            this.originallyTotalPlayersCount = game.players.length;
            this.bannedCards = game.gameSetup.bannedCards;
            this.roundsToWin = game.gameSetup.roundsToWin;
            this.matchingCardStealsTurn = game.gameSetup.matchingCardStealsTurn;
            this.reverseShouldSkipTurnInTwoPlayers = game.gameSetup.reverseShouldSkipTurnInTwoPlayers;
            this.drawAutoPlay = game.gameSetup.drawAutoPlay;
            this.spectatorsCanViewHands = game.gameSetup.spectatorsCanViewHands;
            this.wildCardCanBePlayedOnlyIfNoOtherOptions = game.gameSetup.wildCardCanBePlayedOnlyIfNoOtherOptions;
            this.limitColorChangingCards = game.gameSetup.limitColorChangingCards;
            this.numberOfStandardDecks = game.gameSetup.numberOfStandardDecks;
          }
        });
    } else {
      this._tournamentStorageService.activeTournament
        .pipe(takeWhile(() => this._isAlive))
        .subscribe(tournament => {
          this.readyPlayersLeft = tournament.readyPlayersLeft;
          if (this.originallyTotalPlayersCount == 0) {
            this.originallyTotalPlayersCount = tournament.contestants.length;
            this.bannedCards = tournament.tournamentSetup.bannedCards;
            this.roundsToWin = tournament.tournamentSetup.roundsToWin;
            this.matchingCardStealsTurn = tournament.tournamentSetup.matchingCardStealsTurn;
            this.reverseShouldSkipTurnInTwoPlayers = tournament.tournamentSetup.reverseShouldSkipTurnInTwoPlayers;
            this.drawAutoPlay = tournament.tournamentSetup.drawAutoPlay;
            this.spectatorsCanViewHands = tournament.tournamentSetup.spectatorsCanViewHands;
            this.wildCardCanBePlayedOnlyIfNoOtherOptions = tournament.tournamentSetup.wildCardCanBePlayedOnlyIfNoOtherOptions;
            this.limitColorChangingCards = tournament.tournamentSetup.limitColorChangingCards;
            this.numberOfStandardDecks = tournament.tournamentSetup.numberOfStandardDecks;
          }
        });
    }
  }

  ready() {
    this.isReady = true;
    if (this.isTournament) {
      this._hubService.sendIsReadyForTournament();
    } else {
      this._hubService.sendIsReadyForGame();
    }
  }
  notReady() {
    this._activeModal.dismiss();
  }

  getProgressBarValue() {
    var numberOfPlayersReady =
      this.originallyTotalPlayersCount - this.readyPlayersLeft.length;
    return Math.floor(
      (numberOfPlayersReady / this.originallyTotalPlayersCount) * 100
    );
  }

  getBannedCardName(bannedCard: CardValue) {
    return this._utilityService.getBannedCardName(bannedCard);
  }

  ngOnDestroy(): void {
    this._isAlive = false;
    clearTimeout(this._interval);
  }
}
