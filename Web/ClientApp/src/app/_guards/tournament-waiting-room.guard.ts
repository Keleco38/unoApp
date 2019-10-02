import { TournamentStorageService } from './../_services/storage-services/tournament-storage.service';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class TournamentWaitingRoomGuard implements CanActivate {
  constructor(private _router: Router, private _tournamentStorageService: TournamentStorageService) {}
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this._tournamentStorageService.activeTournament.pipe(
      map(tournament => {
        if (tournament !== null && tournament.tournamentStarted === false) {
          return true;
        } else {
          this._router.navigate(['/']);
        }
      })
    );
  }
}
