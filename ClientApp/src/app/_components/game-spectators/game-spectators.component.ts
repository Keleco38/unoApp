import { Component, OnInit } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { Game } from 'src/app/_models/game';

@Component({
  selector: 'app-game-spectators',
  templateUrl: './game-spectators.component.html',
  styleUrls: ['./game-spectators.component.css']
})
export class GameSpectatorsComponent implements OnInit {
  game: Game;

  constructor(private _hubService: HubService) {}

  ngOnInit() {
    this._hubService.activeGame.subscribe(game => {
      this.game = game;
    });

}
