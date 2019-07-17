import { DigCardComponent } from './../_components/_modals/dig-card/dig-card.component';
import { ShowHandComponent } from './../_components/_modals/show-hand/show-hand.component';
import { Hand } from '../_models/hand';
import { GameMode, CardColor } from './../_models/enums';
import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { BehaviorSubject } from 'rxjs';
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

  private _onlineUsersObservable = new BehaviorSubject<User[]>(new Array<User>());
  private _currentUserObservable = new BehaviorSubject<User>(null);
  private _allChatMessagesObservable = new BehaviorSubject<ChatMessage>(null);
  private _availableGamesObservable = new BehaviorSubject<Game[]>(new Array<Game>());
  private _activeGameObservable = new BehaviorSubject<Game>(null);
  private _gameChatMessagesObservable = new BehaviorSubject<ChatMessage>(null);
  private _myHandObservable = new BehaviorSubject<Hand>(null);

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
      this._allChatMessagesObservable.next(message);
    });

    this._hubConnection.on('PostNewMessageInGameChat', (message: ChatMessage) => {
      this._gameChatMessagesObservable.next(message);
    });

    this._hubConnection.on('RefreshAllGamesList', (games: Game[]) => {
      this._availableGamesObservable.next(games);
    });

    this._hubConnection.on('BuzzPlayer', () => {
      const alert = new Audio('/sounds/alert.mp3');
      alert.load();
      alert.play();
    });

    this._hubConnection.on('KickPlayerFromGame', () => {
      this._activeGameObservable.next(null);
      this._router.navigateByUrl('home');
    });

    this._hubConnection.on('DisplayToastMessage', (message: string) => {
      this._toastrService.info(message, '', { timeOut: 8000 });
    });

    this._hubConnection.on('UpdateMyHand', (myHand: Hand) => {
      this._myHandObservable.next(myHand);
    });

    this._hubConnection.on('ShowInspectedHand', (hand: Hand) => {
      const modalRef = this._modalService.open(ShowHandComponent);
      modalRef.componentInstance.hand = hand;
    });

    this._hubConnection.on('ShowDiscardedPile', (cards: Card[]) => {
      const modalRef = this._modalService.open(DigCardComponent, { backdrop: 'static' });
      modalRef.componentInstance.cards = cards;
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
    this._hubConnection.invoke('SendMessageToAllChat', this._currentUserObservable.getValue().name, message, TypeOfMessage.chat);
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
    this._myHandObservable.next(null);
    this._activeGameObservable.next(null);
    this._hubConnection.invoke('JoinGame', id, password);
  }

  drawCard(count: number, changeTurn: boolean) {
    this._hubConnection.invoke('DrawCard', this._activeGameObservable.getValue().gameSetup.id, count, changeTurn);
  }

  sendMessageToGameChat(message: string): any {
    this._hubConnection.invoke(
      'SendMessageToGameChat',
      this._activeGameObservable.getValue().gameSetup.id,
      this._currentUserObservable.getValue().name,
      message,
      TypeOfMessage.chat
    );
  }

  playCard(card: Card, pickedCardColor: CardColor, playerToSwapCards = '') {
    this._hubConnection.invoke('PlayCard', this._activeGameObservable.getValue().gameSetup.id, card, pickedCardColor, playerToSwapCards);
  }

  digCardFromDiscardedPile(card: Card) {
    this._hubConnection.invoke('DigCardFromDiscardedPile', this._activeGameObservable.getValue().gameSetup.id, card);
  }

  createGame(gameMode: GameMode) {
    this._hubConnection.invoke('CreateGame', gameMode);
  }

  kickPlayerFromGame(user: User): any {
    this._hubConnection.invoke('KickPlayerFromGame', user.name, this._activeGameObservable.getValue().gameSetup.id);
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

  get myHand() {
    return this._myHandObservable.asObservable();
  }
}
