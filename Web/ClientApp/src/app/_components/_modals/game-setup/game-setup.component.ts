import { takeWhile } from 'rxjs/operators';
import { GameType } from './../../../_models/enums';
import { GameSetup } from './../../../_models/gameSetup';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { HubService } from './../../../_services/hub.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Game } from 'src/app/_models/game';

@Component({
  selector: 'app-game-setup',
  templateUrl: './game-setup.component.html',
  styleUrls: ['./game-setup.component.css']
})
export class GameSetupComponent implements OnInit, OnDestroy {
  private _isAlive: boolean = true;
  private _game: Game;
  gameSetup: GameSetup;

  constructor(private _hubService: HubService, private _activeModal: NgbActiveModal) {}

  ngOnInit() {
    this._hubService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe((game: Game) => {
      this._game = game;
    });

    if (this._game === null) {
      this.gameSetup = {
        roundsToWin: 2,
        maxNumberOfPlayers: 6,
        reverseShouldSkipTurnInTwoPlayers: true,
        password: '',
        gameType: GameType.specialWildCards,
        drawFourDrawTwoShouldSkipTurn: true,
        bannedCards: [],
        matchingCardStealsTurn: true,
        wildCardCanBePlayedOnlyIfNoOtherOptions:false
      };
    } else {
      this.gameSetup = this._game.gameSetup;
    }
  }

  closeModal() {
    this._activeModal.dismiss();
  }

  confirm() {
    if (this._game === null) {
      this._hubService.createGame(this.gameSetup);
    } else {
      this._hubService.updateGameSetup(this._game.id, this.gameSetup);
    }
    this._activeModal.close();
  }

  ngOnDestroy(): void {
    console.log('destroyed');
    this._isAlive = false;
  }
}
