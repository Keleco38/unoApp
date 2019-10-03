import { TournamentSetup } from './../_models/tournamentSetup';
import { GameEndedResult } from './../_models/gameEndedResult';
import { GameSetup } from './../_models/gameSetup';
import { UtilityService } from './utility.service';
import { CardColor, ChatDestination } from './../_models/enums';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Subject } from 'rxjs';
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
  private _hubConnection: signalR.HubConnection;

  private _updateCurrentUserObservable = new Subject<User>();
  private _updateOnlineUsersObservable = new Subject<User[]>();
  private _updateAvailableGamesObservable = new Subject<GameList[]>();
  private _updateAvailableTournamentsObservable = new Subject<TournamentList[]>();
  private _updateGameChatMessagesObservable = new Subject<ChatMessage>();
  private _updateRetrieveFullGameChatMessagesObservable = new Subject<ChatMessage[]>();
  private _updateRetrieveFullTournamentChatMessagesObservable = new Subject<ChatMessage[]>();
  private _updateRetrieveFullGameLogObservable = new Subject<string[]>();
  private _updateTournamentChatMessagesObservable = new Subject<ChatMessage>();
  private _updateAllChatMessagesObservable = new Subject<ChatMessage>();
  private _updateGameLogObservable = new Subject<string>();
  private _updateActiveGameObservable = new Subject<Game>();
  private _updateActiveTournamentObservable = new Subject<Tournament>();
  private _updateMyHandObservable = new Subject<Card[]>();
  private _updateMustCallUnoObservable = new Subject();
  private _updateReconnectObservable = new Subject();
  private _updateGameStartedObservable = new Subject();
  private _updateTournamentStartedObservable = new Subject();
  private _updateExitGameObservable = new Subject();
  private _updateExitTournamentObservable = new Subject();
  private _updateShowCardsObservable = new Subject<KeyValue<string, Card[]>[]>();
  private _updateRenameUserObservable = new Subject();
  private _updateShowReadyPhasePlayersObservable = new Subject<boolean>();
  private _updateShowReadyPhaseSpectatorsObservable = new Subject<boolean>();
  private _updateGameEndedObservable = new Subject<GameEndedResult>();

  constructor(private _router: Router, private _toastrService: ToastrService, private _utilityService: UtilityService) {
    this._hubConnection = new signalR.HubConnectionBuilder().withUrl('/gamehub').build();
    if (!environment.production) {
      this._hubConnection.serverTimeoutInMilliseconds = 10000000;
    }

    this._hubConnection.onclose(async () => {
      if (this._wasKicked) return;
      this.startConnection(true);
    });

    this._hubConnection.on('GameEnded', (gameEndedResult: GameEndedResult) => {
      this._updateGameEndedObservable.next(gameEndedResult);
    });

    this._hubConnection.on('ExitGame', () => {
      this._updateExitGameObservable.next();
    });

    this._hubConnection.on('ExitTournament', () => {
      this._updateExitTournamentObservable.next();
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
      this._updateOnlineUsersObservable.next(onlineUsers);
    });

    this._hubConnection.on('UpdateCurrentUser', (user: User) => {
      this._updateCurrentUserObservable.next(user);
    });

    this._hubConnection.on('RenamePlayer', () => {
      if (!environment.production) {
        const myArray = ['Ante', 'Mate', 'Jure', 'Ivica', 'John', 'Bruno', 'Mike', 'David', 'Mokki'];
        var name = myArray[Math.floor(Math.random() * myArray.length)];
        localStorage.setItem('name', name);
        this.addOrRenameUser(name);
        return;
      }
      this._updateRenameUserObservable.next();
    });

    this._hubConnection.on('PostNewMessageInAllChat', (message: ChatMessage) => {
      this._updateAllChatMessagesObservable.next(message);
    });

    this._hubConnection.on('PostNewMessageInTournamentChat', (message: ChatMessage) => {
      this._updateTournamentChatMessagesObservable.next(message);
    });

    this._hubConnection.on('PostNewMessageInGameChat', (message: ChatMessage) => {
      this._updateGameChatMessagesObservable.next(message);
    });

    this._hubConnection.on('RetrieveFullGameChat', (messages: ChatMessage[]) => {
      this._updateRetrieveFullGameChatMessagesObservable.next(messages);
    });

    this._hubConnection.on('RetrieveFullGameLog', (log: string[]) => {
      this._updateRetrieveFullGameLogObservable.next(log);
    });

    this._hubConnection.on('RetrieveFullTournamentChat', (messages: ChatMessage[]) => {
      this._updateRetrieveFullTournamentChatMessagesObservable.next(messages);
    });

    this._hubConnection.on('AddToGameLog', (message: string) => {
      this._updateGameLogObservable.next(message);
    });

    this._hubConnection.on('MustCallUno', () => {
      this._updateMustCallUnoObservable.next();
    });

    this._hubConnection.on('RefreshAllGamesList', (games: GameList[]) => {
      this._updateAvailableGamesObservable.next(games);
    });

    this._hubConnection.on('RefreshAllTournamentsList', (tournaments: TournamentList[]) => {
      this._updateAvailableTournamentsObservable.next(tournaments);
    });

    this._hubConnection.on('BuzzPlayer', (buzzType: string) => {
      this.buzzPlayer(buzzType, false);
    });

    this._hubConnection.on('SendToTheLobby', () => {
      this._router.navigateByUrl('/');
    });

    this._hubConnection.on('StartModalPhasePlayers', (isTournament: boolean) => {
      this._updateShowReadyPhasePlayersObservable.next(isTournament);
    });

    this._hubConnection.on('StartModalPhaseSpectators', (isTournament: boolean) => {
      this._updateShowReadyPhaseSpectatorsObservable.next(isTournament);
    });

    this._hubConnection.on('DisplayToastMessage', (message: string, toastrType: string) => {
      this._toastrService[toastrType](message);
    });

    this._hubConnection.on('GameStarted', () => {
      this._updateGameStartedObservable.next();
    });

    this._hubConnection.on('TournamentStarted', () => {
      this._updateTournamentStartedObservable.next();
    });

    this._hubConnection.on('UpdateMyHand', (myCards: Card[]) => {
      this._updateMyHandObservable.next(myCards);
    });

    this._hubConnection.on('BuzzMyTurnToPlay', () => {
      if (this._utilityService.userSettings.notifyUserWhenHisTurnToPlay) {
        this.buzzPlayer('ding', true);
      }
    });

    this._hubConnection.on('AdminKickUser', () => {
      this._wasKicked = true;
      this._router.navigateByUrl('/');
      this._hubConnection.stop();
      document.body.innerHTML = '<h1>You were temporarily kicked from the server</h1>';
    });

    this._hubConnection.on('ShowCards', (cardsAndNames: KeyValue<string, Card[]>[], showImmediately: boolean) => {
      var delay = showImmediately ? 0 : 2000;
      setTimeout(() => {
        this._updateShowCardsObservable.next(cardsAndNames);
      }, delay);
    });

    this._hubConnection.on('UpdateTournament', (tournament: Tournament) => {
      this._updateActiveTournamentObservable.next(tournament);
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
      this._updateActiveGameObservable.next(game);
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

  private buzzPlayer(buzzType: string, forceBuzz: boolean) {
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

  startConnection(isReconnect: Boolean) {
    if (this._hubConnection.state == 'Connected') {
      return;
    }
    try {
      this._hubConnection.start().then(() => {
        var name = localStorage.getItem('name');
        if (!environment.production) {
          const myArray = ['Ante', 'Mate', 'Jure', 'Ivica', 'John', 'Bruno', 'Mike', 'David', 'Mokki'];
          name = myArray[Math.floor(Math.random() * myArray.length)];
          localStorage.setItem('name', name);
        }

        this.addOrRenameUser(name);

        if (isReconnect) {
          this._updateActiveGameObservable.next(null);
          this._updateActiveTournamentObservable.next(null);
          this._router.navigateByUrl('/');
          this._updateReconnectObservable.next();
        }
      });
    } catch (err) {
      if (environment.production) {
        setTimeout(() => this.startConnection(true), 5000);
      }
    }
  }

  adminKickUser(user: User, password: string) {
    this._hubConnection.invoke('AdminKickUser', user.name, password);
  }

  sendIsReadyForGame() {
    this._hubConnection.invoke('ReadyForGame');
  }

  sendIsReadyForTournament() {
    this._hubConnection.invoke('ReadyForTournament');
  }

  sendMessage(message: string, chatDestination: ChatDestination) {
    if (message == '') return;
    this._hubConnection.invoke('SendMessage', message, chatDestination);
  }

  addOrRenameUser(name: string) {
    this._hubConnection.invoke('AddOrRenameUser', name);
    this._hubConnection.invoke('GetAllGames');
    this._hubConnection.invoke('GetAllOnlineUsers');
    this._hubConnection.invoke('GetAllTournaments');
  }

  joinGame(id: string, password: string): any {
    this._updateMyHandObservable.next(null);
    this._hubConnection.invoke('JoinGame', id, password);
  }

  joinTournament(tournamentId: string, password: string): any {
    this._hubConnection.invoke('JoinTournament', tournamentId, password);
  }

  startTournament(): any {
    this._hubConnection.invoke('StartTournament');
  }

  exitTournament(): any {
    this._hubConnection.invoke('ExitTournament');
  }

  drawCard() {
    this._hubConnection.invoke('DrawCard');
  }

  changeTeam(teamNumber: number) {
    this._hubConnection.invoke('ChangeTeam', teamNumber);
  }

  checkUnoCall(unoCalled: boolean) {
    this._hubConnection.invoke('CheckUnoCall', unoCalled);
  }

  seeTeammatesCards() {
    this._hubConnection.invoke('ShowTeammatesHand');
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

  createGame(gameSetup: GameSetup) {
    this._hubConnection.invoke('CreateGame', gameSetup);
  }

  createTournament(tournamentSetup: TournamentSetup, adminPassword: string) {
    this._hubConnection.invoke('CreateTournament', tournamentSetup, adminPassword);
  }

  kickPlayerFromGame(isBan: boolean, name: string): any {
    this._hubConnection.invoke('KickPlayerFromGame', isBan, name);
  }

  kickContestantFromTournament(isBan: boolean, name: string): any {
    this._hubConnection.invoke('KickContestantFromTournament', isBan, name);
  }

  updateGameSetup(gameSetup: GameSetup) {
    this._hubConnection.invoke('UpdateGameSetup', gameSetup);
  }

  updateTournamentSetup(tournamentSetup: TournamentSetup) {
    this._hubConnection.invoke('UpdateTournamentSetup', tournamentSetup);
  }

  exitGame(): any {
    this._hubConnection.invoke('ExitGame');
  }

  startGame(): any {
    this._hubConnection.invoke('StartGame');
  }

  get updateOnlineUsers() {
    return this._updateOnlineUsersObservable.asObservable();
  }

  get updateCurrentUser() {
    return this._updateCurrentUserObservable.asObservable();
  }

  get updateAllChatMessages() {
    return this._updateAllChatMessagesObservable.asObservable();
  }

  get updateAvailableGames() {
    return this._updateAvailableGamesObservable.asObservable();
  }

  get updateAvailableTournaments() {
    return this._updateAvailableTournamentsObservable.asObservable();
  }

  get updateActiveGame() {
    return this._updateActiveGameObservable.asObservable();
  }

  get updateActiveTournament() {
    return this._updateActiveTournamentObservable.asObservable();
  }

  get updateGameChatMessages() {
    return this._updateGameChatMessagesObservable.asObservable();
  }

  get updateRetrieveFullGameChatMessages() {
    return this._updateRetrieveFullGameChatMessagesObservable.asObservable();
  }

  get updateRetrieveFullTournamentChatMessages() {
    return this._updateRetrieveFullTournamentChatMessagesObservable.asObservable();
  }

  get updateRetrieveFullGameLog() {
    return this._updateRetrieveFullGameLogObservable.asObservable();
  }
  get updateGameLog() {
    return this._updateGameLogObservable.asObservable();
  }

  get updateMyHand() {
    return this._updateMyHandObservable.asObservable();
  }
  get updateMustCallUno() {
    return this._updateMustCallUnoObservable.asObservable();
  }
  get updateOnReconnect() {
    return this._updateReconnectObservable.asObservable();
  }

  get updateTournamentChatMessages() {
    return this._updateTournamentChatMessagesObservable.asObservable();
  }
  get updateGameStarted() {
    return this._updateGameStartedObservable.asObservable();
  }
  get updateTournamentStarted() {
    return this._updateTournamentStartedObservable.asObservable();
  }

  get updateExitGame() {
    return this._updateExitGameObservable.asObservable();
  }
  get updateExitTournament() {
    return this._updateExitTournamentObservable.asObservable();
  }

  get updateShowCards() {
    return this._updateShowCardsObservable.asObservable();
  }
  get updateRenameUser() {
    return this._updateRenameUserObservable.asObservable();
  }
  get updateShowReadyPhasePlayers() {
    return this._updateShowReadyPhasePlayersObservable.asObservable();
  }
  get updateShowReadyPhaseSpectators() {
    return this._updateShowReadyPhaseSpectatorsObservable.asObservable();
  }
  get updateGameEnded() {
    return this._updateGameEndedObservable.asObservable();
  }
}
