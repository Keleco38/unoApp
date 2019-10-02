import { Injectable, OnDestroy } from '@angular/core';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { BehaviorSubject, Subject } from 'rxjs';
import { Tournament } from 'src/app/_models/tournament';
import { HubService } from '../hub.service';
import { takeWhile } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TournamentStorageService implements OnDestroy{
  private _isAlive = true;

  private _tournamentChatMessages: ChatMessage[] = [];
  
  private _tournamentChatMessagesObservable = new BehaviorSubject<ChatMessage[]>([]);
  private _activeTournamentObservable = new BehaviorSubject<Tournament>(null);

  constructor(private _hubService:HubService) {
    this._hubService.updateTournamentChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(message => {
      this._tournamentChatMessages.unshift(message);
      this._tournamentChatMessagesObservable.next(this._tournamentChatMessages);
    });
    this._hubService.updateActiveTournament.pipe(takeWhile(() => this._isAlive)).subscribe(tournament => {
      this._activeTournamentObservable.next(tournament);
    });
    this._hubService.updateRetrieveFullTournamentChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(messages => {
      this._tournamentChatMessages = messages;
      this._tournamentChatMessagesObservable.next(this._tournamentChatMessages);
    });
    this._hubService.updateExitTournament.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this._tournamentChatMessages = [];
      this._tournamentChatMessagesObservable.next(this._tournamentChatMessages);
      this._activeTournamentObservable.next(null);
    });
  }

  get tournamentChatMessages(){
    return this._tournamentChatMessagesObservable.asObservable();
  }

  get activeTournament(){
    return this._activeTournamentObservable.asObservable();
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }


}
