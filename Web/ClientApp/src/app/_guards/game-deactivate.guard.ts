import { TournamentStorageService } from './../_services/storage-services/tournament-storage.service';
import { GameStorageService } from './../_services/storage-services/game-storage.service';
import { GameComponent } from './../_components/game/game.component';
import { HubService } from './../_services/hub.service';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router, CanDeactivate } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable, forkJoin } from 'rxjs';
import { WaitingRoomComponent } from '../_components/waiting-room/waiting-room.component';
import { map } from 'rxjs/operators';
import { Game } from '../_models/game';

@Injectable()
export class GameDeactivateGuard implements CanDeactivate<GameComponent> {
  constructor(
    private _hubService: HubService,
    private _router: Router,
    private _gameStorageService: GameStorageService,
    private _tournamentStorageService: TournamentStorageService
  ) {}
  canDeactivate(
    component: GameComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot
  ): boolean | Observable<boolean> | Promise<boolean> {
    this._hubService.exitGame();

    return this._gameStorageService.activeGame.pipe(
      map((game:Game) => {
        if (game.isTournamentGame) {
          if (nextState.url != '/tournament') {
            this._hubService.exitTournament();
            this._router.navigateByUrl('/');
          }
        }
        return true;
      })
    );
  }
}
