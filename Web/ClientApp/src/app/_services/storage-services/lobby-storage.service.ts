import { StickyTournament } from './../../_models/stickyTournament';
import { HubService } from 'src/app/_services/hub.service';
import { Injectable, OnDestroy } from '@angular/core';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { BehaviorSubject, Subject } from 'rxjs';
import { GameList } from 'src/app/_models/gameList';
import { User } from 'src/app/_models/user';
import { TournamentList } from 'src/app/_models/tournamentList';
import { takeWhile } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class LobbyStorageService implements OnDestroy {
  private _isAlive = true;

  private _allChatMessages: ChatMessage[] = [];
  
  private _onlineUsersObservable = new BehaviorSubject<User[]>([]);
  private _availableGamesObservable = new BehaviorSubject<GameList[]>([]);
  private _availableTournamentsObservable = new BehaviorSubject<TournamentList[]>([]);
  private _allChatMessagesObservable = new BehaviorSubject<ChatMessage[]>([]);
  private _stickyTournaments = new BehaviorSubject<StickyTournament[]>([]);
  private _reconnectObservable = new Subject();

  constructor(private _hubService:HubService) {
    this._hubService.updateAllChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(message => {
      this._allChatMessages.unshift(message);
      this._allChatMessagesObservable.next(this._allChatMessages);
    });
    this._hubService.updateOnlineUsers.pipe(takeWhile(() => this._isAlive)).subscribe(users => {
      this._onlineUsersObservable.next(users);
    });
    this._hubService.updateAvailableGames.pipe(takeWhile(() => this._isAlive)).subscribe(games => {
      this._availableGamesObservable.next(games);
    });
    this._hubService.updateAvailableTournaments.pipe(takeWhile(() => this._isAlive)).subscribe(tournament => {
      this._availableTournamentsObservable.next(tournament);
    });
    this._hubService.updateOnReconnect.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this._reconnectObservable.next();
    });
    this._hubService.updateStickyTournaments.pipe(takeWhile(() => this._isAlive)).subscribe((stickyTournaments) => {
      this._stickyTournaments.next(stickyTournaments);
    });
  }

  get onlineUsers() {
    return this._onlineUsersObservable.asObservable();
  }
  
  get stickyTournaments() {
    return this._stickyTournaments.asObservable();
  }

  get availableGames() {
    return this._availableGamesObservable.asObservable();
  }

  get availableTournaments() {
    return this._availableTournamentsObservable.asObservable();
  }

  get allChatMessages() {
    return this._allChatMessagesObservable.asObservable();
  }

  get userReconnected() {
    return this._reconnectObservable.asObservable();
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
