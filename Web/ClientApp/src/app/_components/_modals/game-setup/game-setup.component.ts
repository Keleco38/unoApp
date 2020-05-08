import { ToastrService } from 'ngx-toastr';
import { UtilityService } from 'src/app/_services/utility.service';
import { GameStorageService } from './../../../_services/storage-services/game-storage.service';
import { first } from 'rxjs/operators';
import { GameType, PlayersSetup, CardValue } from './../../../_models/enums';
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

  constructor(private _hubService: HubService, private _activeModal: NgbActiveModal, private _gameStorageService: GameStorageService, private _utilityService: UtilityService, private _taostrService: ToastrService) { }

  ngOnInit() {
    this._gameStorageService.activeGame.pipe(first()).subscribe((game: Game) => {
      this._game = JSON.parse(JSON.stringify(game));
    });

    if (this._game === null) {
      var lastSetup = this._utilityService.getLastGameSetup();
      if (lastSetup != null) {
        this.gameSetup = lastSetup;
      } else {
        this.restoreDefaults(false);
      }
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
      this._hubService.updateGameSetup(this.gameSetup);
    }
    this._utilityService.setLastGameSetup(this.gameSetup);
    this._activeModal.close();
  }

  restoreDefaults(askConfirm: boolean) {
    if (askConfirm) {
      var cfr = confirm("Really restore default settings?");
      if (!cfr)
        return;
      this._taostrService.info("Default settings have been restored.")
    }

    this.gameSetup = {
      roundsToWin: 2,
      maxNumberOfPlayers: 6,
      reverseShouldSkipTurnInTwoPlayers: true,
      password: '',
      gameType: GameType.specialWildCards,
      drawFourDrawTwoShouldSkipTurn: true,
      bannedCards: [CardValue.swapHands, CardValue.paradigmShift, CardValue.magneticPolarity, CardValue.doubleDraw],
      matchingCardStealsTurn: true,
      wildCardCanBePlayedOnlyIfNoOtherOptions: false,
      playersSetup: PlayersSetup.individual,
      canSeeTeammatesHandInTeamGame: true,
      drawAutoPlay: false,
      spectatorsCanViewHands: true,
      limitColorChangingCards: false,
      numberOfStandardDecks: 4
    };
  }
}
