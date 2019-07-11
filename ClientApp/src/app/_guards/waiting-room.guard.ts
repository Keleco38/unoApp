import { HubService } from './../_services/hub.service';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Injectable()
export class WaitingRoomGuard implements CanActivate {
  constructor(private _hubService: HubService, private _router: Router) {}
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this._hubService.activeGame.pipe(
      map(activeGame => {
        if (activeGame === null) {
          this._router.navigate(['/home']);
          return false;
        } else {
          return true;
        }
      })
    );
  }
}
