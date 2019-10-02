import { GameComponent } from './../_components/game/game.component';
import { HubService } from './../_services/hub.service';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router, CanDeactivate } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { WaitingRoomComponent } from '../_components/waiting-room/waiting-room.component';
import { map } from 'rxjs/operators';

@Injectable()
export class GameDeactivateGuard implements CanDeactivate<GameComponent> {
  constructor(private _hubService: HubService, private _router: Router) {}
  canDeactivate(
    component: GameComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot
  ): boolean | Observable<boolean> | Promise<boolean> {
    this._hubService.exitGame();

    return this._hubService.updateActiveTournament.pipe(
      map(tournament => {
        if (tournament !== null) {
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
