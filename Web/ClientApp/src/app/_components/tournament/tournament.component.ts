import { UserStorageService } from './../../_services/storage-services/user-storage.service';
import { TournamentStorageService } from './../../_services/storage-services/tournament-storage.service';
import { Router } from '@angular/router';
import { UtilityService } from 'src/app/_services/utility.service';
import { Game } from 'src/app/_models/game';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Tournament } from 'src/app/_models/tournament';
import { HubService } from 'src/app/_services/hub.service';
import { takeWhile } from 'rxjs/operators';
import { SidebarSettings } from 'src/app/_models/sidebarSettings';
import { User } from 'src/app/_models/user';
import domtoimage from 'dom-to-image';


@Component({
  selector: 'app-tournament',
  templateUrl: './tournament.component.html',
  styleUrls: ['./tournament.component.css']
})
export class TournamentComponent implements OnInit, OnDestroy {
  private _isAlive: boolean = true;
  sidebarSettings: SidebarSettings;
  activeTournament: Tournament;
  currentUser: User;

  constructor(private _hubService: HubService, private _utilityService: UtilityService, private _router: Router, private _tournamentStorageService: TournamentStorageService, private _userStorageService: UserStorageService) { }

  ngOnInit() {
    this.sidebarSettings = this._utilityService.sidebarSettings;
    this._tournamentStorageService.activeTournament.pipe(takeWhile(() => this._isAlive)).subscribe(tournament => {
      this.activeTournament = tournament;
    });

    this._userStorageService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });
  }

  export() {
    var node = document.getElementById('full-tournament-info');
    var isDarkTheme = this._utilityService.userSettings.useDarkTheme;
    var bgcolor = isDarkTheme ? "#222" : "#fff";
    domtoimage.toJpeg(node, { bgcolor: bgcolor })
      .then(function (dataUrl) {
        var link = document.createElement('a');
        link.download = 'uno-tournament-' + new Date() + '.jpeg';
        link.href = dataUrl;
        link.click();
      });
  }

  exitTournament() {
    this._router.navigateByUrl('/');
  }

  joinGame(game: Game) {
    this._hubService.joinGame(game.id, '');
  }

  getPlayerFromTheGameAtPosition(game: Game, position: number) {
    return game.players.find(x => x.positionInGame == position);
  }

  getExtraPointsWon() {
    return this.activeTournament.contestants.length * this.activeTournament.tournamentSetup.roundsToWin;
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
