import { Component, OnInit, OnDestroy } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
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

  constructor(private _hubService: HubService) {}

  ngOnInit() {
    this._hubService.updateActiveGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      this.game = game;
    });
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
