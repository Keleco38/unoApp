import { GameStorageService } from './../_services/storage-services/game-storage.service';
import { HubService } from './../_services/hub.service';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class GameGuard implements CanActivate {
  constructor(private _hubService: HubService, private _router: Router, private _gameStorageService:GameStorageService) {}
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this._gameStorageService.activeGame.pipe(
      map(activeGame => {
        if (activeGame !== null && activeGame.gameStarted === true) {
          return true;
        } else {
          this._router.navigate(['/']);
        }
      })
    );
  }
}
