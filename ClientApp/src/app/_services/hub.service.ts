import { UtilityService } from './utility.service';
import { DigCardComponent } from './../_components/_modals/dig-card/dig-card.component';
import { ShowCardsComponent } from './../_components/_modals/show-cards/show-cards.component';
import { CardColor, CardValue } from './../_models/enums';
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

  private _currentUserObservable = new BehaviorSubject<User>({} as User);
  private _onlineUsersObservable = new BehaviorSubject<User[]>([]);
  private _availableGamesObservable = new BehaviorSubject<Game[]>([]);
  private _gameChatMessagesObservable = new BehaviorSubject<ChatMessage[]>([]);
  private _gameChatNumberOfMessagesObservable = new Subject<ChatMessage>();
  private _allChatMessagesObservable = new BehaviorSubject<ChatMessage[]>([]);
  private _gameLogObservable = new BehaviorSubject<string[]>([]);
  private _activeGameObservable = new BehaviorSubject<Game>(null);
  private _myHandObservable = new BehaviorSubject<Card[]>([]);
  private _mustCallUnoObservable = new Subject();
  private _reconnectObservable = new Subject();

  private async startConnection(isReconnect: Boolean) {
    try {
      await this._hubConnection.start().then(() => {
        this.addOrRenameUser(false);
        if (isReconnect) {
          this._router.navigateByUrl('/');
          this._activeGameObservable.next(null);
          this._reconnectObservable.next();
        }
      });
    } catch (err) {
      setTimeout(() => this.startConnection(true), 5000);
    }
  }

  constructor(
    private _router: Router,
    private _toastrService: ToastrService,
    private _modalService: NgbModal,
    private _utilityService: UtilityService
  ) {
    this._hubConnection = new signalR.HubConnectionBuilder().withUrl('/gamehub').build();
    this.startConnection(false);

    this._hubConnection.onclose(async () => {
      await this.startConnection(true);
    });

    this._hubConnection.on('ExitGame', () => {
      this._gameChatMessages = [];
      this._gameLog = [];
      this._gameLogObservable.next(this._gameLog);
      this._gameChatMessagesObservable.next(this._gameChatMessages);
      this._activeGameObservable.next(null);
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
      this._gameChatNumberOfMessagesObservable.next(message);
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
      this.buzzPlayer(buzzType, false);
    });

    this._hubConnection.on('KickPlayerFromGame', () => {
      this._toastrService.info('You have been kicked from the game');
      this._router.navigateByUrl('/');
    });

    this._hubConnection.on('DisplayToastMessage', (message: string) => {
      this._toastrService.info(message, '', { timeOut: 3000 });
    });

    this._hubConnection.on('UpdateMyHand', (myCards: Card[]) => {
      this._myHandObservable.next(myCards);
    });

    this._hubConnection.on('ShowCards', (cards: Card[]) => {
      setTimeout(() => {
        const modalRef = this._modalService.open(ShowCardsComponent, { backdrop: 'static' });
        modalRef.componentInstance.cards = cards;
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

      if (this._utilityService.userSettings.notifyUserWhenHisTurnToPlay) {
        if (game.playerToPlay.user.name == this._currentUserObservable.getValue().name) {
          this.buzzPlayer('ding', true);
        }
      }
    });
  }

  sendMessageToAllChat(message: string, isBuzz: boolean) {
    if (message == '') return;
    if (isBuzz) {
      if (this._activeGameObservable.getValue() != null) {
        this._hubConnection.invoke('SendMessage', message, this._activeGameObservable.getValue().id);
        return;
      }
    }
    this._hubConnection.invoke('SendMessage', message, '');
  }
  sendMessageToGameChat(message: string) {
    if (message == '') return;
    this._hubConnection.invoke('SendMessage', message, this._activeGameObservable.getValue().id);
  }

  addOrRenameUser(forceRename: boolean) {
    let name;
    if (environment.production) {
      do {
        if (forceRename) {
          name = prompt("Your name is already taken or it's not set. Please input a new name (only letters and numbers allowed):");
        } else {
          name =
            localStorage.getItem('name') ||
            prompt("Your name is already taken or it's not set. Please input a new name (only letters and numbers allowed):");
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
    this._hubConnection.invoke('JoinGame', id, password);
  }

  drawCard(count: number, changeTurn: boolean) {
    this._hubConnection.invoke('DrawCard', this._activeGameObservable.getValue().id, count, changeTurn);
  }

  playCard(
    cardPlayedId: string,
    targetedCardColor: CardColor = 0,
    targetedPlayer: string = '',
    cardToDigId: string = '',
    duelNumbers: number[] = null,
    charityCardsIds: string[] = null,
    blackjackNumber: number = 0,
    numbersToDiscard: number[] = null,
    cardPromisedToDiscardId: string = '',
    oddOrEvenGuess: string = ''
  ) {
    this._hubConnection.invoke(
      'PlayCard',
      this._activeGameObservable.getValue().id,
      cardPlayedId,
      targetedCardColor,
      targetedPlayer,
      cardToDigId,
      duelNumbers,
      charityCardsIds,
      blackjackNumber,
      numbersToDiscard,
      cardPromisedToDiscardId,
      oddOrEvenGuess
    );
  }

  digCardFromDiscardedPile(card: Card) {
    this._hubConnection.invoke('DigCardFromDiscardedPile', this._activeGameObservable.getValue().id, card);
  }

  createGame() {
    this._gameChatMessages = [];
    this._gameLog = [];
    this._myHandObservable.next(null);
    this._hubConnection.invoke('CreateGame');
  }

  kickPlayerFromGame(user: User): any {
    this._hubConnection.invoke('KickPlayerFromGame', user.name, this._activeGameObservable.getValue().id);
  }

  updateGameSetup(id: string, bannedCards: CardValue[], roundsToWin: number) {
    this._hubConnection.invoke('UpdateGameSetup', id, bannedCards, roundsToWin);
  }

  exitGame(): any {
    if (!this._activeGameObservable.getValue()) {
      return;
    }
    this._hubConnection.invoke('ExitGame', this._activeGameObservable.getValue().id);
    this._gameChatMessages = [];
    this._gameLog = [];
    this._gameLogObservable.next(this._gameLog);
    this._gameChatMessagesObservable.next(this._gameChatMessages);
    this._activeGameObservable.next(null);
  }

  startGame(): any {
    this._hubConnection.invoke('StartGame', this._activeGameObservable.getValue().id);
  }

  setGamePassword(id: string, roomPassword: string): any {
    this._hubConnection.invoke('SetGamePassword', id, roomPassword);
  }

  buzzPlayer(buzzType: string, forceBuzz: boolean) {
    var index = this._utilityService.userSettings.blockedBuzzCommands.indexOf(buzzType);
    if (forceBuzz) {
      index = -1;
    }
    if (index == -1) {
      const alert = new Audio(`/sounds/${buzzType}.mp3`);
      alert.load();
      alert.play();
    }
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
  get gameChatNumberOfMessages() {
    return this._gameChatNumberOfMessagesObservable.asObservable();
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
  get onReconnect() {
    return this._reconnectObservable.asObservable();
  }
}
