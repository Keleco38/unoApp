import { HubService } from './../_services/hub.service';
import { RouterStateSnapshot, CanDeactivate, ActivatedRouteSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { WaitingRoomComponent } from '../_components/waiting-room/waiting-room.component';

@Injectable()
export class WaitingRoomDeactivateGuard implements CanDeactivate<WaitingRoomComponent> {
  constructor(private _hubService: HubService) {}

  canDeactivate(
    component: WaitingRoomComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot
  ): boolean | Observable<boolean> | Promise<boolean> {
    if (nextState.url === '/game') {
      return true;
    }

    this._hubService.exitGame();
    return true;
  }
}
