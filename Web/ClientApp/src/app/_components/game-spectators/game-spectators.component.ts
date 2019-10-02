import { GameStorageService } from './../../_services/storage-services/game-storage.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Game } from 'src/app/_models/game';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-game-spectators',
  templateUrl: './game-spectators.component.html',
  styleUrls: ['./game-spectators.component.css']
})
export class GameSpectatorsComponent implements OnInit, OnDestroy {
  private _isAlive: boolean = true;
  game: Game;

  constructor(private _gameStorageService: GameStorageService) {}

  ngOnInit() {
    this._gameStorageService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      this.game = game;
    });
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
