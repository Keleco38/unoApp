import { LobbyStorageService } from 'src/app/_services/storage-services/lobby-storage.service';
import { TournamentStorageService } from './../../_services/storage-services/tournament-storage.service';
import { GameStorageService } from 'src/app/_services/storage-services/game-storage.service';
import { UserStorageService } from './../../_services/storage-services/user-storage.service';
import { ModalSubscribingService } from './../../_services/modal-subscribing.service';
import { ModalService } from '../../_services/modal.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { takeWhile } from 'rxjs/operators';
import { UtilityService } from 'src/app/_services/utility.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  private _isAlive = true;

  userReconnected: boolean = false;

  constructor(
    private _hubService: HubService,
    private _utilityService: UtilityService,
    private _modalService: ModalService,
    private _router: Router,
    activateMSS: ModalSubscribingService,
    activateUSS: UserStorageService,
    activateGSS: GameStorageService,
    activateTSS: TournamentStorageService,
    activateLSS: LobbyStorageService,
  ) { }

  ngOnInit(): void {
    this._hubService.updateOnReconnect.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this.userReconnected = true;
    });
    var isFirstTimeLunched = this._utilityService.isFirstTimeLunched;
    if (isFirstTimeLunched) {
      this._utilityService.updateFirstTimeLunched();
      var firstTimeLaunchedModal = this._modalService.displayFirstTimeLaunchedModal();
      firstTimeLaunchedModal.result.then((sendToHelpPage: boolean) => {
        if (sendToHelpPage) {
          this._router.navigateByUrl('/help');
        }
      });
    }
  }

  createTournament() {
    this._modalService.displayTournamentSetupModal();
  }

  createGame() {
    this._modalService.displayGameSetupModal();
  }

  removeAlert() {
    this.userReconnected = false;
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
