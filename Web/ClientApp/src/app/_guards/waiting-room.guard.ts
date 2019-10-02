import { GameStorageService } from './../_services/storage-services/game-storage.service';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class WaitingRoomGuard implements CanActivate {
  constructor(private _gameStorageService: GameStorageService, private _router: Router) {}
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this._gameStorageService.activeGame.pipe(
      map(activeGame => {
        if (activeGame !== null && activeGame.gameStarted === false) {
          return true;
        } else {
          this._router.navigate(['/']);
        }
      })
    );
  }
}
