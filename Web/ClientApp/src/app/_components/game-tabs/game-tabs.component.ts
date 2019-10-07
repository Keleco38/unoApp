import { GameStorageService } from './../../_services/storage-services/game-storage.service';
import { TournamentStorageService } from './../../_services/storage-services/tournament-storage.service';
import { Game } from 'src/app/_models/game';
import { Tournament } from 'src/app/_models/tournament';
import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-game-tabs',
  templateUrl: './game-tabs.component.html',
  styleUrls: ['./game-tabs.component.css']
})
export class GameTabsComponent implements OnInit {
  @Input('sidebarChatHeight') sidebarChatHeight: number;

  private _isAlive: boolean = true;
  activeGame: Game;
  activeTournament: Tournament;

  constructor(private _tournamentStorageService: TournamentStorageService, private _gameStorageService: GameStorageService) {}

  ngOnInit() {
    this._gameStorageService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      this.activeGame = game;
    });
    this._tournamentStorageService.activeTournament.pipe(takeWhile(() => this._isAlive)).subscribe(activeTournament => {
      this.activeTournament = activeTournament;
    });
  }

  getMiscTabClass() {
    var miscHeight = parseInt(this.sidebarChatHeight.toString());
    return `fill-viewport-${miscHeight + 10}`;
  }

  getMiscHeight() {
    return parseInt(this.sidebarChatHeight.toString()) + 8;
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
