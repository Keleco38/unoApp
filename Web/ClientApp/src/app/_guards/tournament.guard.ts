import { TournamentStorageService } from './../_services/storage-services/tournament-storage.service';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class TournamentGuard implements CanActivate {
  constructor(private _router: Router, private tournamentStorageService:TournamentStorageService) {}
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this.tournamentStorageService.activeTournament.pipe(
      map(tournament => {
        if (tournament !== null && tournament.tournamentStarted === true) {
          return true;
        } else {
          this._router.navigate(['/']);
        }
      })
    );
  }
}
