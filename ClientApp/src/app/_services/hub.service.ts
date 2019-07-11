import { GameMode } from './../_models/enums';
import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { BehaviorSubject } from 'rxjs';
import { User } from '../_models/user';
import { TypeOfMessage } from '../_models/enums';
import { ChatMessage } from '../_models/chatMessage';
import { Game } from '../_models/game';

@Injectable({
  providedIn: 'root'
})
export class HubService {
  private _hubConnection: signalR.HubConnection;
  private _allChatMessages: ChatMessage[] = [];
  private _buzzPlayerDisabled: boolean;
  private _onlineUsersObservable = new BehaviorSubject<User[]>(new Array<User>());
  private _currentUserObservable = new BehaviorSubject<User>(null);
  private _allChatMessagesObservable = new BehaviorSubject<ChatMessage[]>(this._allChatMessages);
  private _availableGamesObservable = new BehaviorSubject<Game[]>(new Array<Game>());

  constructor(private _router: Router) {
    this._hubConnection = new signalR.HubConnectionBuilder().withUrl('/gamehub').build();
    this._hubConnection.start().then(() => {
      this.rename(false);
    });

    this._hubConnection.on('RefreshOnlineUsersList', (onlineUsers: User[]) => {
      this._onlineUsersObservable.next(onlineUsers);
    });

    this._hubConnection.on('UpdateCurrentUser', (user: User) => {
      this._currentUserObservable.next(user);
    });

    this._hubConnection.on('RenamePlayer', () => {
      this.rename(true);
    });

    this._hubConnection.on('PostNewMessageInAllChat', (message: ChatMessage) => {
      this._allChatMessages.unshift(message);
      this._allChatMessagesObservable.next(this._allChatMessages);
    });
    this._hubConnection.on('RefreshAllGamesList', (games: Game[]) => {
      this._availableGamesObservable.next(games);
    });
    this._hubConnection.on('BuzzPlayer', () => {
      if (this._buzzPlayerDisabled) {
        return;
      }
      this._buzzPlayerDisabled = true;
      const alert = new Audio('/sounds/alert.mp3');
      alert.load();
      alert.play();
      setTimeout(() => {
        this._buzzPlayerDisabled = false;
      }, 5000);
    });
  }

  sendMessageToAllChat(message: string): any {
    this._hubConnection.invoke('SendMessageToAllChat', this._currentUserObservable.getValue().name, message, TypeOfMessage.chat);
  }

  rename(forceRename: boolean) {
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
    this._hubConnection.invoke('JoinGame', id, password);
  }

  createGame(gameMode:GameMode) {
    this._hubConnection.invoke('CreateGame', gameMode);
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
}
