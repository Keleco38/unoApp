import { Component, OnInit, OnDestroy } from '@angular/core';
import { Game } from 'src/app/_models/game';
import { HubService } from 'src/app/_services/hub.service';
import { takeWhile } from 'rxjs/operators';
import { GameList } from 'src/app/_models/gameList';

@Component({
  selector: 'app-available-games',
  templateUrl: './available-games.component.html',
  styleUrls: ['./available-games.component.css']
})
export class AvailableGamesComponent implements OnInit, OnDestroy {
  ngOnDestroy(): void {
    this._isAlive = false;
  }
  private _isAlive: boolean = true;
  availableGames: GameList[] = new Array<GameList>();

  constructor(private _hubService: HubService) {}

  ngOnInit(): void {
    this._hubService.availableGames.pipe(takeWhile(() => this._isAlive)).subscribe((availableGames: GameList[]) => {
      this.availableGames = availableGames;
    });
  }

  joinGame(game: GameList) {
    let password = '';

    if (game.isPasswordProtected) {
      password = prompt('Input password for this game');
      if (password == null) {
        return;
      }
    }
    this._hubService.joinGame(game.id, password);
  }
}
