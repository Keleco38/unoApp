import { Router } from '@angular/router';
import { UtilityService } from 'src/app/_services/utility.service';
import { Game } from 'src/app/_models/game';
import { Component, OnInit } from '@angular/core';
import { Tournament } from 'src/app/_models/tournament';
import { HubService } from 'src/app/_services/hub.service';
import { takeWhile } from 'rxjs/operators';
import { SidebarSettings } from 'src/app/_models/sidebarSettings';

@Component({
  selector: 'app-tournament',
  templateUrl: './tournament.component.html',
  styleUrls: ['./tournament.component.css']
})
export class TournamentComponent implements OnInit {
  private _isAlive: boolean = true;
  sidebarSettings: SidebarSettings;
  activeTournament: Tournament;

  constructor(private _hubService: HubService, private _utilityService:UtilityService, private _router:Router) {}

  ngOnInit() {
    this.sidebarSettings = this._utilityService.sidebarSettings;
    this._hubService.activeTournament.pipe(takeWhile(() => this._isAlive)).subscribe(tournament => {
      this.activeTournament = tournament;
    });
  }


  exitTournament(){
   this._router.navigateByUrl("/");
  }

  joinGame(game: Game) {
    this._hubService.joinGame(game.id, '');
  }

  
  ngOnDestroy(): void {
    this._isAlive = false;
  }


}
