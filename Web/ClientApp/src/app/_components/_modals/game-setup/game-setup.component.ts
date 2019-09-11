import { first } from 'rxjs/operators';
import { GameType, PlayersSetup } from './../../../_models/enums';
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
export class GameSetupComponent implements OnInit {
  private _game: Game;
  gameSetup: GameSetup;

  constructor(private _hubService: HubService, private _activeModal: NgbActiveModal) {}

  ngOnInit() {
    this._hubService.activeGame.pipe(first()).subscribe((game: Game) => {
      this._game = JSON.parse(JSON.stringify(game));
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
        wildCardCanBePlayedOnlyIfNoOtherOptions: false,
        playersSetup: PlayersSetup.individual
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
}
