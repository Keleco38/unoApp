import { HubService } from 'src/app/_services/hub.service';
import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { User } from 'src/app/_models/user';
import { takeWhile } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserStorageService implements OnDestroy {
  private _isAlive = true;
  private _currentUserObservable = new BehaviorSubject<User>({} as User);

  constructor(private _hubService: HubService) {
    this._hubService.updateCurrentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this._currentUserObservable.next(user);
    });
  }

  get currentUser(){
    return this._currentUserObservable.asObservable();
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
