import { Component, OnInit } from '@angular/core';
import { Game } from 'src/app/_models/game';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';
import { Router } from '@angular/router';
import { Player } from 'src/app/_models/player';
import { GameMode } from 'src/app/_models/enums';

@Component({
  selector: 'app-waiting-room',
  templateUrl: './waiting-room.component.html',
  styleUrls: ['./waiting-room.component.css']
})
export class WaitingRoomComponent implements OnInit {
  activeGame: Game;
  password: string;
  currentUser: User;

  constructor(private _hubService: HubService, private _router: Router) {}

  ngOnInit() {
    this._hubService.activeGame.subscribe(game => {
      this.activeGame = game;
    });

    this._hubService.currentUser.subscribe(user => {
      this.currentUser = user;
    });
  }

  leaveWaitingRoom() {
    this._hubService.exitGame();
  }

  joinGame() {
    this._hubService.joinGame(this.activeGame.gameSetup.id, '');
  }

  userIsSpectator() {
    const exists = this.activeGame.spectators.find(spectator => {
      return spectator.user.name === this.currentUser.name;
    });
    return exists != null;
  }

  startGame() {
    this._hubService.startGame();
  }

  setRoomPassword() {
    this._hubService.setGamePassword(this.activeGame.gameSetup.id, this.password);
    this.password = '';
  }

  kickPlayerFromGame(player: Player) {
    const cfrm = confirm('Really kick this player? ' + player.user.name);
    if (cfrm) {
      this._hubService.kickPlayerFromGame(player.user);
    }
  }
  updateGameSetup() {
    this._hubService.updateGameSetup(this.activeGame.gameSetup.id, this.activeGame.gameSetup.gameMode, this.activeGame.gameSetup.roundsToWin);
  }
  getGameModeText() {
    switch (this.activeGame.gameSetup.gameMode) {
      case GameMode.normal:
        return 'Normal';
      case GameMode.specialCards:
        return 'Special cards';
      case GameMode.specialCardsAndAvalonCards:
        return 'Special cards + Avalon cards';
    }
  }
}
