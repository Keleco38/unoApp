import { HubService } from 'src/app/_services/hub.service';
import { Game } from './../../../_models/game';
import { Component, OnInit } from '@angular/core';
import { Direction, GameMode } from 'src/app/_models/enums';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-game-info',
  templateUrl: './game-info.component.html',
  styleUrls: ['./game-info.component.css']
})
export class GameInfoComponent implements OnInit {
  game: Game;
  gameLog: string[];

  constructor(private _activeModal: NgbActiveModal, private _hubService: HubService) {}

  ngOnInit() {
    this._hubService.activeGame.subscribe(game => {
      this.game = game;
    });

    this._hubService.gameLog.subscribe(gameLog => {
      this.gameLog = gameLog;
    });
  }
  closeModal() {
    this._activeModal.dismiss();
  }

  getGameModeText() {
    switch (this.game.gameSetup.gameMode) {
      case GameMode.normal:
        return 'Normal';
      case GameMode.specialCards:
        return 'Special cards';
      case GameMode.specialCardsAndAvalonCards:
        return 'Special cards + Avalon cards';
    }
  }
}
