import { ModalObservablesService } from './modal-services/modal-observables.service';
import { ModalService } from 'src/app/_services/modal-services/modal.service';
import { TournamentSetup } from './../_models/tournamentSetup';
import { GameEndedResult } from './../_models/gameEndedResult';
import { GameSetup } from './../_models/gameSetup';
import { UtilityService } from './utility.service';
import { CardColor, ChatDestination } from './../_models/enums';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, Subject } from 'rxjs';
import { User } from '../_models/user';
import { ChatMessage } from '../_models/chatMessage';
import { Game } from '../_models/game';
import { ToastrService } from 'ngx-toastr';
import { Card } from '../_models/card';
import { GameList } from '../_models/gameList';
import { Tournament } from '../_models/tournament';
import { TournamentList } from '../_models/tournamentList';
import { KeyValue } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class HubService {
  private _wasKicked = false;
   _hubConnection: signalR.HubConnection;
  private _gameChatMessages: ChatMessage[] = [];
  private _tournamentChatMessages: ChatMessage[] = [];
  private _gameLog: string[] = [];
  private _allChatMessages: ChatMessage[] = [];
  private _currentUserObservable = new BehaviorSubject<User>({} as User);
  private _onlineUsersObservable = new BehaviorSubject<User[]>([]);
  private _availableGamesObservable = new BehaviorSubject<GameList[]>([]);
  private _availableTournamentsObservable = new BehaviorSubject<TournamentList[]>([]);
  private _gameChatMessagesObservable = new BehaviorSubject<ChatMessage[]>([]);
  private _tournamentChatMessagesObservable = new BehaviorSubject<ChatMessage[]>([]);
  private _gameChatNumberOfMessagesObservable = new Subject<ChatMessage>();
  private _allChatMessagesObservable = new BehaviorSubject<ChatMessage[]>([]);
  private _gameLogObservable = new BehaviorSubject<string[]>([]);
  private _activeGameObservable = new BehaviorSubject<Game>(null);
  private _activeTournamentObservable = new BehaviorSubject<Tournament>(null);
  private _myHandObservable = new BehaviorSubject<Card[]>([]);
  private _mustCallUnoObservable = new Subject();
  private _reconnectObservable = new Subject();
  private _gameStartedObservable = new Subject();
  private _tournamentStartedObservable = new Subject();

  private async startConnection(isReconnect: Boolean) {
    try {
      this._hubConnection.start().then(() => {
        this.addOrRenameUser(false);
        if (isReconnect) {
          this._activeGameObservable.next(null);
          this._activeTournamentObservable.next(null);
          this._router.navigateByUrl('/');
          this._reconnectObservable.next();
        }
      });
    } catch (err) {
      if (environment.production) {
        setTimeout(() => this.startConnection(true), 5000);
      }
    }
  }

  constructor(
    private _router: Router,
    private _toastrService: ToastrService,
    private _modalObservableService: ModalObservablesService,
    private _utilityService: UtilityService
  ) {
    this._hubConnection = new signalR.HubConnectionBuilder().withUrl('/gamehub').build();
    if (!environment.production) {
      this._hubConnection.serverTimeoutInMilliseconds = 10000000;
    }
    this.startConnection(false);

    this._hubConnection.onclose(async () => {
      if (this._wasKicked) return;
      this.startConnection(true);
    });

    this._hubConnection.on('GameEnded', (gameEndedResult: GameEndedResult) => {
      this._modalObservableService.gameEndedResultModalObservable.next(gameEndedResult);
    });

    this._hubConnection.on('ExitGame', () => {
      this._gameChatMessages = [];
      this._gameLog = [];
      this._gameLogObservable.next(this._gameLog);
      this._gameChatMessagesObservable.next(this._gameChatMessages);
      this._activeGameObservable.next(null);
    });

    this._hubConnection.on('ExitTournament', () => {
      this._tournamentChatMessages = [];
      this._tournamentChatMessagesObservable.next(this._tournamentChatMessages);
      this._activeTournamentObservable.next(null);
    });

    this._hubConnection.on('UserMentioned', () => {
      var notifyWhenMentionedToast = this._utilityService.userSettings.notifyWhenMentionedToast;
      var notifyWhenMentionedBuzz = this._utilityService.userSettings.notifyWhenMentionedBuzz;

      if (notifyWhenMentionedToast) {
        this._toastrService.success('You were mentioned in chat');
      }
      if (notifyWhenMentionedBuzz) {
        this.buzzPlayer('ding', true);
      }
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

    this._hubConnection.on('PostNewMessageInTournamentChat', (message: ChatMessage) => {
      this._tournamentChatMessages.unshift(message);
      this._tournamentChatMessagesObservable.next(this._tournamentChatMessages);
    });

    this._hubConnection.on('PostNewMessageInGameChat', (message: ChatMessage) => {
      this._gameChatMessages.unshift(message);
      this._gameChatNumberOfMessagesObservable.next(message);
      this._gameChatMessagesObservable.next(this._gameChatMessages);
    });

    this._hubConnection.on('RetrieveFullGameChat', (messages: ChatMessage[]) => {
      this._gameChatMessages = messages;
      this._gameChatMessagesObservable.next(this._gameChatMessages);
    });

    this._hubConnection.on('RetrieveFullGameLog', (log: string[]) => {
      this._gameLog = log;
      this._gameLogObservable.next(this._gameLog);
    });

    this._hubConnection.on('RetrieveFullTournamentChat', (messages: ChatMessage[]) => {
      this._tournamentChatMessages = messages;
      this._tournamentChatMessagesObservable.next(this._tournamentChatMessages);
    });

    this._hubConnection.on('AddToGameLog', (message: string) => {
      this._gameLog.unshift(message);
      this._gameLogObservable.next(this._gameLog);
    });

    this._hubConnection.on('MustCallUno', () => {
      this._mustCallUnoObservable.next();
    });

    this._hubConnection.on('RefreshAllGamesList', (games: GameList[]) => {
      this._availableGamesObservable.next(games);
    });

    this._hubConnection.on('RefreshAllTournamentsList', (tournaments: TournamentList[]) => {
      this._availableTournamentsObservable.next(tournaments);
    });

    this._hubConnection.on('BuzzPlayer', (buzzType: string) => {
      this.buzzPlayer(buzzType, false);
    });

    this._hubConnection.on('KickPlayerFromGame', () => {
      this._toastrService.info('You have been kicked from the game');
      this._router.navigateByUrl('/');
    });

    this._hubConnection.on('KickContestantFromTournament', () => {
      this._toastrService.info('You have been kicked from the tournament');
      this._router.navigateByUrl('/');
    });

    this._hubConnection.on('DisplayReadyModalPlayers', (isTournament: boolean) => {
      this._modalObservableService.readyPhasePlayersModalObservable.next(isTournament);
    });

    this._hubConnection.on('DisplayReadyModalSpectators', (isTournament: boolean) => {
      this._modalObservableService.readyPhaseSpectatorsModalObservable.next(isTournament);
    });

    this._hubConnection.on('DisplayToastMessage', (message: string, toastrType: string) => {
      this._toastrService[toastrType](message);
    });

    this._hubConnection.on('GameStarted', () => {
      this._gameStartedObservable.next();
    });

    this._hubConnection.on('TournamentStarted', () => {
      this._tournamentStartedObservable.next();
    });

    this._hubConnection.on('UpdateMyHand', (myCards: Card[]) => {
      this._myHandObservable.next(myCards);
      if (this._utilityService.userSettings.notifyUserWhenHisTurnToPlay) {
        if (this._activeGameObservable.getValue().playerToPlay.user.name == this._currentUserObservable.getValue().name) {
          this.buzzPlayer('ding', true);
        }
      }
    });

    this._hubConnection.on('AdminKickUser', () => {
      this._wasKicked = true;
      this._router.navigateByUrl('/');
      this._activeGameObservable.next(null);
      this._activeTournamentObservable.next(null);
      this._hubConnection.stop();
      document.body.innerHTML = '<h1>You were temporarily kicked from the server</h1>';
    });

    this._hubConnection.on('ShowCards', (cardsAndNames: KeyValue<string, Card[]>[], showImmediately: boolean) => {
      var delay = showImmediately ? 0 : 2000;
      setTimeout(() => {
        this._modalObservableService.showCardsModalObservable.next(cardsAndNames);
      }, delay);
    });

    this._hubConnection.on('UpdateTournament', (tournament: Tournament) => {
      this._activeTournamentObservable.next(tournament);
      if (tournament.tournamentStarted) {
        if (this._router.url !== '/tournament' && this._router.url !== '/game') {
          this._router.navigateByUrl('/tournament');
        }
      } else {
        if (this._router.url !== '/tournament-waiting-room') {
          this._router.navigateByUrl('/tournament-waiting-room');
        }
      }
    });

    this._hubConnection.on('UpdateGame', (game: Game) => {
      this._activeGameObservable.next(game);
      if (game.gameStarted) {
        if (this._router.url !== '/game') {
          this._router.navigateByUrl('/game');
        }
      } else {
        if (this._router.url !== '/waiting-room') {
          this._router.navigateByUrl('/waiting-room');
        }
      }
    });
  }

  adminKickUser(user: User, password: string) {
    this._hubConnection.invoke('AdminKickUser', user.name, password);
  }

  sendIsReadyForGame() {
    this._hubConnection.invoke('ReadyForGame', this._activeGameObservable.getValue().id);
  }

  sendIsReadyForTournament() {
    this._hubConnection.invoke('ReadyForTournament', this._activeTournamentObservable.getValue().id);
  }

  sendMessage(message: string, chatDestination: ChatDestination) {
    if (message == '') return;
    var gameId = this._activeGameObservable.getValue() != null ? this._activeGameObservable.getValue().id : '';
    var tournamentId = this._activeTournamentObservable.getValue() != null ? this._activeTournamentObservable.getValue().id : '';
    this._hubConnection.invoke('SendMessage', message, chatDestination, gameId, tournamentId);
  }

  addOrRenameUser(forceRename: boolean) {
    this._modalObservableService.renameModalObservable.next(forceRename);
  }

  joinGame(id: string, password: string): any {
    this._myHandObservable.next(null);
    this._hubConnection.invoke('JoinGame', id, password);
  }

  joinTournament(id: string, password: string): any {
    this._hubConnection.invoke('JoinTournament', id, password);
  }

  startTournament(): any {
    if (!this._activeTournamentObservable.getValue()) {
      return;
    }
    this._hubConnection.invoke('StartTournament', this._activeTournamentObservable.getValue().id);
  }

  exitTournament(): any {
    if (!this._activeTournamentObservable.getValue()) {
      return;
    }
    this._hubConnection.invoke('ExitTournament', this._activeTournamentObservable.getValue().id);
  }

  drawCard() {
    this._hubConnection.invoke('DrawCard', this._activeGameObservable.getValue().id);
  }

  changeTeam(teamNumber: number) {
    this._hubConnection.invoke('ChangeTeam', this._activeGameObservable.getValue().id, teamNumber);
  }

  checkUnoCall(unoCalled: boolean) {
    this._hubConnection.invoke('CheckUnoCall', this._activeGameObservable.getValue().id, unoCalled);
  }

  seeTeammatesCards() {
    this._hubConnection.invoke('ShowTeammatesHand', this._activeGameObservable.getValue().id);
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

  createGame(gameSetup: GameSetup) {
    this._gameChatMessages = [];
    this._gameLog = [];
    this._myHandObservable.next(null);
    this._hubConnection.invoke('CreateGame', gameSetup);
  }

  createTournament(tournamentSetup: TournamentSetup, adminPassword: string) {
    this._hubConnection.invoke('CreateTournament', tournamentSetup, adminPassword);
  }

  kickPlayerFromGame(user: User): any {
    this._hubConnection.invoke('KickPlayerFromGame', user.name, this._activeGameObservable.getValue().id);
  }

  kickContestantFromTournament(user: User): any {
    this._hubConnection.invoke('KickContestantFromTournament', user.name, this._activeTournamentObservable.getValue().id);
  }

  updateGameSetup(gameId: string, gameSetup: GameSetup) {
    this._hubConnection.invoke('UpdateGameSetup', gameId, gameSetup);
  }

  updateTournamentSetup(tournamentId: string, tournamentSetup: TournamentSetup) {
    this._hubConnection.invoke('UpdateTournamentSetup', tournamentId, tournamentSetup);
  }

  exitGame(): any {
    if (!this._activeGameObservable.getValue()) {
      return;
    }
    this._hubConnection.invoke('ExitGame', this._activeGameObservable.getValue().id);
  }

  startGame(): any {
    this._hubConnection.invoke('StartGame', this._activeGameObservable.getValue().id);
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

  get availableTournaments() {
    return this._availableTournamentsObservable.asObservable();
  }

  get activeGame() {
    return this._activeGameObservable.asObservable();
  }

  get activeTournament() {
    return this._activeTournamentObservable.asObservable();
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

  get tournamentChatMessages() {
    return this._tournamentChatMessagesObservable.asObservable();
  }
  get gameStarted() {
    return this._gameStartedObservable.asObservable();
  }
  get tournamentStarted() {
    return this._tournamentStartedObservable.asObservable();
  }
}
