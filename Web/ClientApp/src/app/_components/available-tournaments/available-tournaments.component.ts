import { LobbyStorageService } from './../../_services/storage-services/lobby-storage.service';
import { Tournament } from './../../_models/tournament';
import { HubService } from './../../_services/hub.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { takeWhile } from 'rxjs/operators';
import { TournamentList } from '../../_models/tournamentList';
import { ModalService } from 'src/app/_services/modal.service';

@Component({
  selector: 'app-available-tournaments',
  templateUrl: './available-tournaments.component.html',
  styleUrls: ['./available-tournaments.component.css']
})
export class AvailableTournamentsComponent implements OnInit, OnDestroy {
  availableTournaments: TournamentList[] = [];
  _isAlive: boolean = true;

  constructor(
    private _hubService: HubService,
    private _lobbyStorageService: LobbyStorageService,
    private _modalService: ModalService
  ) {}

  ngOnInit() {
    this._lobbyStorageService.availableTournaments.pipe(takeWhile(() => this._isAlive)).subscribe(tournaments => {
      this.availableTournaments = tournaments;
    });
  }

  getBadgeClass(tournament: Tournament) {
    if (tournament.tournamentStarted) return 'badge-danger';
    return 'badge-primary';
  }

  joinTournament(tournament: TournamentList) {
    if (tournament.isPasswordProtected) {
      this._modalService.displayInputPasswordModal(
        true,
        tournament.id,
        tournament.name,
        tournament.tournamentStarted,
        tournament.numberOfPlayers,
        tournament.requiredNumberOfPlayers
      );
      return;
    }
    this._hubService.joinTournament(tournament.id, '');
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
