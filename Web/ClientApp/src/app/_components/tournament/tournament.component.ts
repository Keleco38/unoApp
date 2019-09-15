import { Game } from 'src/app/_models/game';
import { Component, OnInit } from '@angular/core';
import { Tournament } from 'src/app/_models/tournament';
import { HubService } from 'src/app/_services/hub.service';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-tournament',
  templateUrl: './tournament.component.html',
  styleUrls: ['./tournament.component.css']
})
export class TournamentComponent implements OnInit {
  private _isAlive: boolean = true;
  activeTournament: Tournament;

  constructor(private _hubService: HubService) {}

  ngOnInit() {
    this._hubService.activeTournament.pipe(takeWhile(() => this._isAlive)).subscribe(tournament => {
      this.activeTournament = tournament;
    });
  }

  joinTournament() {
    this._hubService.joinTournament(this.activeTournament.id, '');
  }

  startTournament() {
    this._hubService.startTournament();
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
  joinGame(game: Game) {
    this._hubService.joinGame(game.id, '');
  }
}
