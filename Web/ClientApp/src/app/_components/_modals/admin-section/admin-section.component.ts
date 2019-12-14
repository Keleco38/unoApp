import { GameStorageService } from './../../../_services/storage-services/game-storage.service';
import { HubService } from 'src/app/_services/hub.service';
import { LobbyStorageService } from './../../../_services/storage-services/lobby-storage.service';
import { User } from 'src/app/_models/user';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { takeWhile } from 'rxjs/operators';
import { TournamentList } from 'src/app/_models/tournamentList';
import { GameList } from 'src/app/_models/gameList';
import { Player } from 'src/app/_models/player';
import { Game } from 'src/app/_models/game';

@Component({
  selector: 'app-admin-section',
  templateUrl: './admin-section.component.html',
  styleUrls: ['./admin-section.component.css']
})
export class AdminSectionComponent implements OnInit, OnDestroy {
  private _isAlive = true;
  password: string;
  onlineUsers: User[];
  availableTournaments: TournamentList[];
  availableGames: GameList[];
  currentGame: Game;

  constructor(private _activeModal: NgbActiveModal, private _lobbyStorageService: LobbyStorageService, private _hubService: HubService, private _gameStorageService:GameStorageService) { }

  ngOnInit() {
    this._lobbyStorageService.onlineUsers.pipe(takeWhile(() => this._isAlive)).subscribe(onlinePlayers => {
      this.onlineUsers = onlinePlayers;
    });
    this._lobbyStorageService.availableTournaments.pipe(takeWhile(() => this._isAlive)).subscribe(availableTournaments => {
      this.availableTournaments = availableTournaments;
    });
    this._lobbyStorageService.availableGames.pipe(takeWhile(() => this._isAlive)).subscribe(availableGames => {
      this.availableGames = availableGames;
    });
    this._gameStorageService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(currentGame => {
      this.currentGame = currentGame;
    });
  }

  closeModal() {
    this._activeModal.dismiss();
  }

  kickUserFromServer(user: User) {
    var cmfr = confirm(`Really kick ${user.name}?`);
    if (cmfr) {
      this._hubService.adminKickUser(user, this.password);
    }
  }

  buzzAll() {
    var cmfr = confirm(`Really buzz all players?`);
    if (cmfr) {
      this._hubService.adminBuzzAll(this.password);
    }
  }

  forceWinGame(player: Player) {
    var cmfr = confirm(`Really force win for the player ${player.user.name}?`);
    if (cmfr) {
      this._hubService.adminForceWinGame(this.password, this.currentGame.id, player.id);
    }
  }

  cleanupGame(game: GameList) {
    var cmfr = confirm(`Really cleanup game ${game.host}?`);
    if (cmfr) {
      this._hubService.adminCleanupGame(game.id, this.password);
    }
  }

  cleanupTournament(tournament: TournamentList) {
    var cmfr = confirm(`Really cleanup tournament ${tournament.name}?`);
    if (cmfr) {
      this._hubService.adminCleanupTournament(tournament.id, this.password);
    }
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
