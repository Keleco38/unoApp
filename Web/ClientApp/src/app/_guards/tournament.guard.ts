import { HubService } from './../_services/hub.service';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Injectable()
export class TournamentGuard implements CanActivate {
  constructor(private _hubService: HubService, private _router: Router) {}
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this._hubService.updateActiveTournament.pipe(
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
