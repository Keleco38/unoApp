import { UtilityService } from './../../../_services/utility.service';
import { CardValue, GameType } from './../../../_models/enums';
import { HubService } from 'src/app/_services/hub.service';
import { Game } from './../../../_models/game';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Direction } from 'src/app/_models/enums';
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

  constructor(private _activeModal: NgbActiveModal, private _hubService: HubService,private _utilityService:UtilityService) {}

  ngOnInit() {
    this._hubService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      this.game = game;
    });

    this._hubService.gameLog.pipe(takeWhile(() => this._isAlive)).subscribe(gameLog => {
      this.gameLog = gameLog;
    });
  }
  closeModal() {
    this._activeModal.dismiss();
  }

  getGameTypePlaceholder() {
    return this.game.gameSetup.gameType == GameType.normal ? 'Normal' : 'Special Wild Cards';
  }

  
  getBannedCardName(bannedCard:CardValue){
    return this._utilityService.getBannedCardName(bannedCard);
  }

  ngOnDestroy(): void {
   this._isAlive=false
  }
 

}
