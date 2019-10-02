import { Injectable, OnDestroy } from '@angular/core';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { BehaviorSubject, Subject } from 'rxjs';
import { Game } from 'src/app/_models/game';
import { Card } from 'src/app/_models/card';
import { HubService } from '../hub.service';
import { takeWhile } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class GameStorageService implements OnDestroy {
  private _isAlive = true;

  private _gameChatMessages: ChatMessage[] = [];
  private _gameLog: string[] = [];

  private _gameChatMessagesObservable = new BehaviorSubject<ChatMessage[]>([]);
  private _gameLogObservable = new BehaviorSubject<string[]>([]);
  private _activeGameObservable = new BehaviorSubject<Game>(null);
  private _myHandObservable = new BehaviorSubject<Card[]>([]);
  private _mustCallUnoObservable = new Subject();
  private _gameChatNumberOfMessagesObservable = new Subject<ChatMessage>();

  constructor(private _hubService: HubService) {
    this._hubService.updateGameChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(message => {
      this._gameChatMessages.unshift(message);
      this._gameChatNumberOfMessagesObservable.next(message);
      this._gameChatMessagesObservable.next(this._gameChatMessages);
    });
    this._hubService.updateGameLog.pipe(takeWhile(() => this._isAlive)).subscribe(log => {
      this._gameLog.unshift(log);
      this._gameLogObservable.next(this._gameLog);
    });
    this._hubService.updateActiveGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      this._activeGameObservable.next(game);
    });
    this._hubService.updateMyHand.pipe(takeWhile(() => this._isAlive)).subscribe(cards => {
      this._myHandObservable.next(cards);
      
    });
    this._hubService.updateMustCallUno.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this._mustCallUnoObservable.next();
    });
    this._hubService.updateRetrieveFullGameChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe((messages) => {
      this._gameChatMessages = messages;
      this._gameChatMessagesObservable.next(this._gameChatMessages);
    });
    this._hubService.updateRetrieveFullGameLog.pipe(takeWhile(() => this._isAlive)).subscribe((log) => {
      this._gameLog = log;
      this._gameLogObservable.next(this._gameLog);
    });
    this._hubService.updateExitGame.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this._gameChatMessages = [];
      this._gameLog = [];
      this._gameLogObservable.next(this._gameLog);
      this._gameChatMessagesObservable.next(this._gameChatMessages);
      this._myHandObservable.next(null);
      this._activeGameObservable.next(null);
    });
  }

  get gameChatNumberOfMessage() {
    return this._gameChatNumberOfMessagesObservable.asObservable();
  }
  get gameChat() {
    return this._gameChatMessagesObservable.asObservable();
  }

  get gameLog() {
    return this._gameLogObservable.asObservable();
  }

  get activeGame() {
    return this._activeGameObservable.asObservable();
  }

  get myHand() {
    return this._myHandObservable.asObservable();
  }

  get mustCallUno() {
    return this._mustCallUnoObservable.asObservable();
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
