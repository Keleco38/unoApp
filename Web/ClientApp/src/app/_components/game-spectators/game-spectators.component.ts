import { UserStorageService } from './../../_services/storage-services/user-storage.service';
import { ModalService } from './../../_services/modal.service';
import { GameStorageService } from './../../_services/storage-services/game-storage.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Game } from 'src/app/_models/game';
import { takeWhile } from 'rxjs/operators';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-game-spectators',
  templateUrl: './game-spectators.component.html',
  styleUrls: ['./game-spectators.component.css']
})
export class GameSpectatorsComponent implements OnInit, OnDestroy {
  private _isAlive: boolean = true;
  game: Game;
  currentUser: User;

  constructor(private _gameStorageService: GameStorageService, private _modalService: ModalService, private _userStorageService:UserStorageService) {}

  ngOnInit() {
    this._gameStorageService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      this.game = game;
    });
    this._userStorageService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });
  }

  openKickBanUserModal(user: User) {
    this._modalService.displayKickBanPlayerModal(false, user);
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
