import { ModalService } from './../../_services/modal.service';
import { LobbyStorageService } from './../../_services/storage-services/lobby-storage.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Game } from 'src/app/_models/game';
import { HubService } from 'src/app/_services/hub.service';
import { takeWhile } from 'rxjs/operators';
import { GameList } from 'src/app/_models/gameList';

@Component({
  selector: 'app-available-games',
  templateUrl: './available-games.component.html',
  styleUrls: ['./available-games.component.css']
})
export class AvailableGamesComponent implements OnInit, OnDestroy {
  ngOnDestroy(): void {
    this._isAlive = false;
  }
  private _isAlive: boolean = true;
  availableGames: GameList[] = new Array<GameList>();

  constructor(
    private _hubService: HubService,
    private _lobbyStorageService: LobbyStorageService,
    private _modalService: ModalService
  ) {}

  ngOnInit(): void {
    this._lobbyStorageService.availableGames.pipe(takeWhile(() => this._isAlive)).subscribe((availableGames: GameList[]) => {
      this.availableGames = availableGames;
    });
  }

  getBadgeClass(game: Game) {
    if (game.gameStarted) return 'badge-danger';
    return 'badge-primary';
  }

  joinGame(game: GameList) {
    if (game.isPasswordProtected) {
      this._modalService.displayInputPasswordModal(false,game.id, game.host, game.gameStarted, game.numberOfPlayers, game.maxNumberOfPlayers);
      return;
    }
    this._hubService.joinGame(game.id, '');
  }
}
