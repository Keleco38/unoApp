import { HubService } from 'src/app/_services/hub.service';
import { Tournament } from 'src/app/_models/tournament';
import { Component, OnInit } from '@angular/core';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-tournament-spectators',
  templateUrl: './tournament-spectators.component.html',
  styleUrls: ['./tournament-spectators.component.css']
})
export class TournamentSpectatorsComponent implements OnInit {

  private _isAlive: boolean = true;
  activeTournament: Tournament;

  constructor(private _hubService: HubService) {}

  ngOnInit() {
    this._hubService.updateActiveTournament.pipe(takeWhile(() => this._isAlive)).subscribe(activeTournament => {
      this.activeTournament = activeTournament;
    });
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }

}
