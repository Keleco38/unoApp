import { Component, OnInit, OnDestroy, Injector, Input } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-ready-phase-spectators',
  templateUrl: './ready-phase-spectators.component.html',
  styleUrls: ['./ready-phase-spectators.component.css']
})
export class ReadyPhaseSpectatorsComponent implements OnInit, OnDestroy {
  @Input('isTournament') isTournament: boolean;

  private _hubService: HubService;
  private _isAlive = true;
  private _interval;
  private _countDown = 10000;

  timer: number = 10;
  readyPlayersLeft: string[];
  originallyTotalPlayersCount: number = 0;

  constructor(private _activeModal: NgbActiveModal, private _toastrService: ToastrService, injector: Injector) {
    this._hubService = injector.get(HubService);
  }

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
      this._hubService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
        this.readyPlayersLeft = game.readyPlayersLeft;
        if (this.originallyTotalPlayersCount == 0) {
          this.originallyTotalPlayersCount = game.players.length;
        }
      });
    } else {
      this._hubService.activeTournament.pipe(takeWhile(() => this._isAlive)).subscribe(tournament => {
        this.readyPlayersLeft = tournament.readyPlayersLeft;
        if (this.originallyTotalPlayersCount == 0) {
          this.originallyTotalPlayersCount = tournament.contestants.length;
        }
      });
    }
  }


  getProgressBarValue() {
    var numberOfPlayersReady = this.originallyTotalPlayersCount - this.readyPlayersLeft.length;
    return (numberOfPlayersReady / this.originallyTotalPlayersCount) * 100;
  }

  ngOnDestroy(): void {
    this._isAlive = false;
    clearTimeout(this._interval);
  }

}
