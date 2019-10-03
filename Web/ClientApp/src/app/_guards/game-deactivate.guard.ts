import { TournamentStorageService } from './../_services/storage-services/tournament-storage.service';
import { GameStorageService } from './../_services/storage-services/game-storage.service';
import { GameComponent } from './../_components/game/game.component';
import { HubService } from './../_services/hub.service';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router, CanDeactivate } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable, forkJoin } from 'rxjs';
import { WaitingRoomComponent } from '../_components/waiting-room/waiting-room.component';
import { map } from 'rxjs/operators';

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
    return forkJoin(this._gameStorageService.activeGame, this._tournamentStorageService.activeTournament).pipe(
      map(([activeGame, tournament]) => {
        if (activeGame !== null) {
          this._hubService.exitGame(activeGame.id);
        }
        if (tournament !== null) {
          if (nextState.url != '/tournament') {
            this._hubService.exitTournament(tournament.id);
            this._router.navigateByUrl('/');
          }
        }
        return true;
      })
    );
  }
}
