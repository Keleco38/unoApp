import { Component, OnInit } from '@angular/core';
import { Game } from 'src/app/_models/game';
import { HubService } from 'src/app/_services/hub.service';

@Component({
  selector: 'app-available-games',
  templateUrl: './available-games.component.html',
  styleUrls: ['./available-games.component.css']
})
export class AvailableGamesComponent implements OnInit {
  availableGames: Game[] = new Array<Game>();

  constructor(private _hubService: HubService) {}

  ngOnInit(): void {
    this._hubService.availableGames.subscribe((availableGames: Game[]) => {
      this.availableGames = availableGames;
    });
  }

  joinGame(game: Game) {
    let password = '';

    if (game.gameSetup.isPasswordProtected) {
      password = prompt('Input password for this game');
      if (password == null) {
        return;
      }
    }
    this._hubService.joinGame(game.gameSetup.id, password);
  }
}
