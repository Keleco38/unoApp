import { TournamentStorageService } from './../../../_services/storage-services/tournament-storage.service';
import { GameStorageService } from './../../../_services/storage-services/game-storage.service';
import { ToastrService } from 'ngx-toastr';
import { HubService } from './../../../_services/hub.service';
import { Component, OnInit, Input, OnDestroy, Injector, EventEmitter, Output } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Game } from 'src/app/_models/game';
import { takeWhile } from 'rxjs/operators';
import { Tournament } from 'src/app/_models/tournament';

@Component({
  selector: 'app-confirm-ready',
  templateUrl: './confirm-ready.component.html',
  styleUrls: ['./confirm-ready.component.css']
})
export class ConfirmReadyComponent implements OnInit, OnDestroy {
  @Input('isTournament') isTournament: boolean;

  private _isAlive = true;
  private _interval;
  private _countDown = 10000;

  private _tournament: Tournament;
  private _game: Game;

  timer: number = 10;
  isReady: boolean = false;
  readyPlayersLeft: string[];
  originallyTotalPlayersCount: number = 0;

  constructor(
    private _activeModal: NgbActiveModal,
    private _toastrService: ToastrService,
    private _hubService: HubService,
    private _gameStorageService: GameStorageService,
    private _tournamentStorageService: TournamentStorageService
  ) {}

  ngOnInit() {
    this._interval = setInterval(() => {
      this._countDown -= 1000;
      this.timer = Math.floor(this._countDown / 1000);
      if (this.timer <= 0) {
        this._toastrService.error(`Players not ready: ${this.readyPlayersLeft.join(', ')}`);
        this._activeModal.dismiss();
      }
    }, 1000);
    if (!this.isTournament) {
      this._gameStorageService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
        this._game = game;
        this.readyPlayersLeft = game.readyPlayersLeft;
        if (this.originallyTotalPlayersCount == 0) {
          this.originallyTotalPlayersCount = game.players.length;
        }
      });
    } else {
      this._tournamentStorageService.activeTournament.pipe(takeWhile(() => this._isAlive)).subscribe(tournament => {
        this._tournament = tournament;
        this.readyPlayersLeft = tournament.readyPlayersLeft;
        if (this.originallyTotalPlayersCount == 0) {
          this.originallyTotalPlayersCount = tournament.contestants.length;
        }
      });
    }
  }

  ready() {
    this.isReady = true;
    if (this.isTournament) {
      this._hubService.sendIsReadyForTournament(this._tournament.id);
    } else {
      this._hubService.sendIsReadyForGame(this._game.id);
    }
  }
  notReady() {
    this._activeModal.dismiss();
  }

  getProgressBarValue() {
    var numberOfPlayersReady = this.originallyTotalPlayersCount - this.readyPlayersLeft.length;
    return Math.floor((numberOfPlayersReady / this.originallyTotalPlayersCount) * 100);
  }

  ngOnDestroy(): void {
    this._isAlive = false;
    clearTimeout(this._interval);
  }
}
