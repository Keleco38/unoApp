import { UtilityService } from 'src/app/_services/utility.service';
import { Tournament } from './../../_models/tournament';
import { HubService } from './../../_services/hub.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { takeWhile } from 'rxjs/operators';
import { TournamentList } from '../../_models/tournamentList';

@Component({
  selector: 'app-available-tournaments',
  templateUrl: './available-tournaments.component.html',
  styleUrls: ['./available-tournaments.component.css']
})
export class AvailableTournamentsComponent implements OnInit, OnDestroy {
  availableTournaments: TournamentList[] = [];
  _isAlive: boolean = true;

  constructor(private _hubService: HubService, private _utilityService: UtilityService) {}

  ngOnInit() {
    this._hubService.updateAvailableTournaments.pipe(takeWhile(() => this._isAlive)).subscribe(tournaments => {
      this.availableTournaments = tournaments;
    });
  }

  getBadgeClass(tournament: Tournament) {
    if (tournament.tournamentStarted) return 'badge-danger';
    return 'badge-primary';
  }

  joinTournament(tournament: TournamentList) {
    let password = '';

    if (tournament.isPasswordProtected) {
      password = prompt('Input password for this tournament');
      if (password == null) {
        return;
      }
    }
    this._hubService.joinTournament(tournament.id, password);
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
