import { DigCardComponent } from './../_components/_modals/dig-card/dig-card.component';
import { ShowHandComponent } from './../_components/_modals/show-hand/show-hand.component';
import { Hand } from '../_models/hand';
import { GameMode, CardColor } from './../_models/enums';
import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, Subject, ReplaySubject } from 'rxjs';
import { User } from '../_models/user';
import { TypeOfMessage } from '../_models/enums';
import { ChatMessage } from '../_models/chatMessage';
import { Game } from '../_models/game';
import { ToastrService } from 'ngx-toastr';
import { Card } from '../_models/card';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Injectable({
  providedIn: 'root'
})
export class HubService {
  private _hubConnection: signalR.HubConnection;
  private _gameChatMessages: ChatMessage[] = [];
  private _gameLog: string[] = [];
  private _allChatMessages: ChatMessage[] = [];

  private _currentUserObservable = new BehaviorSubject<User>(null);
  private _onlineUsersObservable = new ReplaySubject<User[]>(1);
  private _availableGamesObservable = new ReplaySubject<Game[]>(1);
  private _gameChatMessagesObservable = new BehaviorSubject<ChatMessage[]>(this._gameChatMessages);
  private _allChatMessagesObservable = new BehaviorSubject<ChatMessage[]>(this._allChatMessages);
  private _gameLogObservable = new BehaviorSubject<string[]>(this._gameLog);
  private _activeGameObservable = new BehaviorSubject<Game>(null);
  private _myHandObservable = new BehaviorSubject<Hand>(null);
  private _mustCallUnoObservable = new Subject();

  constructor(private _router: Router, private _toastrService: ToastrService, private _modalService: NgbModal) {
    this._hubConnection = new signalR.HubConnectionBuilder().withUrl('/gamehub').build();
    this._hubConnection.start().then(() => {
      this.addOrRenameUser(false);
    });

    this._hubConnection.on('RefreshOnlineUsersList', (onlineUsers: User[]) => {
      this._onlineUsersObservable.next(onlineUsers);
    });

    this._hubConnection.on('UpdateCurrentUser', (user: User) => {
      this._currentUserObservable.next(user);
    });

    this._hubConnection.on('RenamePlayer', () => {
      this.addOrRenameUser(true);
    });

    this._hubConnection.on('PostNewMessageInAllChat', (message: ChatMessage) => {
      this._allChatMessages.unshift(message);
      this._allChatMessagesObservable.next(this._allChatMessages);
    });

    this._hubConnection.on('PostNewMessageInGameChat', (message: ChatMessage) => {
      this._gameChatMessages.unshift(message);
      this._gameChatMessagesObservable.next(this._gameChatMessages);
    });

    this._hubConnection.on('AddToGameLog', (message: string) => {
      this._gameLog.unshift(message);
      this._gameLogObservable.next(this._gameLog);
    });

    this._hubConnection.on('MustCallUno', () => {
      this._mustCallUnoObservable.next();
    });

    this._hubConnection.on('RefreshAllGamesList', (games: Game[]) => {
      this._availableGamesObservable.next(games);
    });

    this._hubConnection.on('BuzzPlayer', (buzzType: string) => {
      const alert = new Audio(`/sounds/${buzzType}.mp3`);
      alert.load();
      alert.play();
    });

    this._hubConnection.on('KickPlayerFromGame', () => {
      this._activeGameObservable.next(null);
      this._router.navigateByUrl('home');
    });

    this._hubConnection.on('DisplayToastMessage', (message: string) => {
      this._toastrService.info(message, '', { timeOut: 3000 });
    });

    this._hubConnection.on('UpdateMyHand', (myHand: Hand) => {
      this._myHandObservable.next(myHand);
    });

    this._hubConnection.on('ShowInspectedHand', (hand: Hand) => {
      setTimeout(() => {
        const modalRef = this._modalService.open(ShowHandComponent);
        modalRef.componentInstance.hand = hand;
      }, 2000);
    });

    this._hubConnection.on('UpdateGame', (game: Game) => {
      this._activeGameObservable.next(game);
      if (game.gameStarted) {
        if (this._router.url !== '/game') {
          this._router.navigateByUrl('/game');
        }
      } else {
        if (this._router.url !== '/waitingRoom') {
          this._router.navigateByUrl('/waitingRoom');
        }
      }
    });
  }

  sendMessageToAllChat(message: string): any {
    this._hubConnection.invoke('SendMessageToAllChat', message, TypeOfMessage.chat);
  }

  addOrRenameUser(forceRename: boolean) {
    let name;
    if (environment.production) {
      do {
        if (forceRename) {
          name = prompt('Input your name');
        } else {
          name = localStorage.getItem('name') || prompt('Input your name');
        }
      } while (!name);
    } else {
      const myArray = ['Ante', 'Mate', 'Jure', 'Ivica', 'John'];
      name = myArray[Math.floor(Math.random() * myArray.length)];
    }
    localStorage.setItem('name', name);
    this._hubConnection.invoke('AddOrRenameUser', name);
    this._hubConnection.invoke('GetAllGames');
  }

  joinGame(id: string, password: string): any {
    this._gameChatMessages = [];
    this._gameLog = [];
    this._myHandObservable.next(null);
    this._hubConnection.invoke('JoinGame', id, password);
  }

  drawCard(count: number, changeTurn: boolean) {
    this._hubConnection.invoke('DrawCard', this._activeGameObservable.getValue().gameSetup.id, count, changeTurn);
  }

  sendMessageToGameChat(message: string): any {
    this._hubConnection.invoke('SendMessageToGameChat', this._activeGameObservable.getValue().gameSetup.id, message, TypeOfMessage.chat);
  }

  playCard(
    card: Card,
    pickedCardColor: CardColor,
    playerToSwapCards: string = '',
    cardToDig: Card = null,
    duelNumbers: number[] = null,
    charityCards: Card[] = null
  ) {
    this._hubConnection.invoke(
      'PlayCard',
      this._activeGameObservable.getValue().gameSetup.id,
      card,
      pickedCardColor,
      playerToSwapCards,
      cardToDig,
      duelNumbers,
      charityCards
    );
  }

  digCardFromDiscardedPile(card: Card) {
    this._hubConnection.invoke('DigCardFromDiscardedPile', this._activeGameObservable.getValue().gameSetup.id, card);
  }

  createGame() {
    this._gameChatMessages = [];
    this._gameLog = [];
    this._myHandObservable.next(null);
    this._hubConnection.invoke('CreateGame');
  }

  kickPlayerFromGame(user: User): any {
    this._hubConnection.invoke('KickPlayerFromGame', user.name, this._activeGameObservable.getValue().gameSetup.id);
  }

  updateGameSetup(id: string, gameMode: GameMode, roundsToWin: number) {
    this._hubConnection.invoke('UpdateGameSetup', id, gameMode, roundsToWin);
  }

  exitGame(): any {
    if (!this._activeGameObservable.getValue()) {
      return;
    }
    this._hubConnection.invoke('ExitGame', this._activeGameObservable.getValue().gameSetup.id);
    this._router.navigate(['/home']);
    this._activeGameObservable.next(null);
  }

  startGame(): any {
    this._hubConnection.invoke('StartGame', this._activeGameObservable.getValue().gameSetup.id);
  }

  setGamePassword(id: string, roomPassword: string): any {
    this._hubConnection.invoke('SetGamePassword', id, roomPassword);
  }

  get onlineUsers() {
    return this._onlineUsersObservable.asObservable();
  }

  get currentUser() {
    return this._currentUserObservable.asObservable();
  }

  get allChatMessages() {
    return this._allChatMessagesObservable.asObservable();
  }

  get availableGames() {
    return this._availableGamesObservable.asObservable();
  }

  get activeGame() {
    return this._activeGameObservable.asObservable();
  }

  get gameChatMessages() {
    return this._gameChatMessagesObservable.asObservable();
  }
  get gameLog() {
    return this._gameLogObservable.asObservable();
  }

  get myHand() {
    return this._myHandObservable.asObservable();
  }
  get mustCallUno() {
    return this._mustCallUnoObservable.asObservable();
  }
}
