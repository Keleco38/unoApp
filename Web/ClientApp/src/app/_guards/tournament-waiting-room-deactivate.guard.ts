import { TournamentStorageService } from './../_services/storage-services/tournament-storage.service';
import { HubService } from './../_services/hub.service';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router, CanDeactivate } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { WaitingRoomComponent } from '../_components/waiting-room/waiting-room.component';
import { map } from 'rxjs/operators';

@Injectable()
export class TournamentWaitingRoomDeactivateGuard implements CanDeactivate<WaitingRoomComponent> {
  constructor(private _hubService: HubService, private _router: Router, private _tournamentStorageService:TournamentStorageService) {}
  canDeactivate(
    component: WaitingRoomComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot
  ): boolean | Observable<boolean> | Promise<boolean> {
    if (nextState.url === '/tournament') {
      return true;
    }
    this._tournamentStorageService.activeTournament.pipe(
      map(tournament => {
        if (tournament != null) {
          this._hubService.exitTournament(tournament.id);
        }
      })
    );
    return true;
  }
}
