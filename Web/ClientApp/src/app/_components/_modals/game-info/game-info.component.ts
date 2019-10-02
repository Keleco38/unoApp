import { GameStorageService } from './../../../_services/storage-services/game-storage.service';
import { UtilityService } from './../../../_services/utility.service';
import { CardValue, GameType, PlayersSetup } from './../../../_models/enums';
import { HubService } from 'src/app/_services/hub.service';
import { Game } from './../../../_models/game';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-game-info',
  templateUrl: './game-info.component.html',
  styleUrls: ['./game-info.component.css']
})
export class GameInfoComponent implements OnInit, OnDestroy {

  private _isAlive: boolean=true;
  game: Game;
  gameLog: string[];

  constructor(private _activeModal: NgbActiveModal,private _gameStorageService:GameStorageService, private _hubService: HubService,private _utilityService:UtilityService) {}

  ngOnInit() {
    this._gameStorageService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      this.game = game;
    });

    this._gameStorageService.gameLog.pipe(takeWhile(() => this._isAlive)).subscribe(gameLog => {
      this.gameLog = gameLog;
    });
  }
  closeModal() {
    this._activeModal.dismiss();
  }

  getGameTypePlaceholder() {
    return this.game.gameSetup.gameType == GameType.normal ? 'Normal' : 'Special Wild Cards';
  }
  
  getPlayerSetupPlaceholder() {
    return this.game.gameSetup.playersSetup == PlayersSetup.individual ? 'Individual' : 'Teams';
  }
  
  getBannedCardName(bannedCard:CardValue){
    return this._utilityService.getBannedCardName(bannedCard);
  }

  ngOnDestroy(): void {
   this._isAlive=false
  }
 

}
