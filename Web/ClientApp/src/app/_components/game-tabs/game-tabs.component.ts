import { HubService } from 'src/app/_services/hub.service';
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
  @Input('heightClassString') heightClassString: string;

  private _isAlive: boolean = true;
  activeGame: Game;
  activeTournament: Tournament;

  constructor(private _hubService: HubService) {}

  ngOnInit() {
    this._hubService.updateActiveGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      this.activeGame = game;
    });
    this._hubService.updateActiveTournament.pipe(takeWhile(() => this._isAlive)).subscribe(activeTournament => {
      this.activeTournament = activeTournament;
    });
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
