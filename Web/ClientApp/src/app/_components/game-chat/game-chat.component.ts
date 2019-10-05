import { UserStorageService } from './../../_services/storage-services/user-storage.service';
import { LobbyStorageService } from './../../_services/storage-services/lobby-storage.service';
import { GameStorageService } from './../../_services/storage-services/game-storage.service';
import { ChatDestination } from './../../_models/enums';
import { SidebarSettings } from './../../_models/sidebarSettings';
import { UtilityService } from './../../_services/utility.service';
import { Component, OnInit, OnDestroy, Input, ElementRef, ViewChild } from '@angular/core';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';
import { TypeOfMessage } from 'src/app/_models/enums';
import { Game } from 'src/app/_models/game';
import { takeWhile, map } from 'rxjs/operators';
import { UserSettings } from 'src/app/_models/userSettings';

@Component({
  selector: 'app-game-chat',
  templateUrl: './game-chat.component.html',
  styleUrls: ['./game-chat.component.css']
})
export class GameChatComponent implements OnInit, OnDestroy {
  @Input('heightClassString') heightClassString: string;
  @ViewChild('messageInput', { static: false }) messageInput: ElementRef;

  private _isAlive: boolean = true;
  onlineUsers: string[];
  messages: ChatMessage[];
  newMessage = '';
  activeGame: Game;
  sidebarSettings: SidebarSettings;
  currentUser: User;
  userSettings: UserSettings;

  constructor(
    private _hubService: HubService,
    private _userStorageService: UserStorageService,
    private _utilityService: UtilityService,
    private _lobbyStorageService: LobbyStorageService,
    private _gameStorageService: GameStorageService
  ) {}

  ngOnInit(): void {
    this.userSettings = this._utilityService.userSettings;
    this._gameStorageService.gameChat.pipe(takeWhile(() => this._isAlive)).subscribe(messages => {
      this.messages = messages;
    });
    this._userStorageService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });
    this._gameStorageService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      this.activeGame = game;
    });
    this._lobbyStorageService.onlineUsers
      .pipe(takeWhile(() => this._isAlive))
      .pipe(
        map(users => {
          return users.map(user => {
            return user.name;
          });
        })
      )
      .subscribe((userNames: string[]) => {
        this.onlineUsers = userNames;
      });
    this.sidebarSettings = this._utilityService.sidebarSettings;
  }

  addEmojiToChat(event) {
    this.newMessage += event;
    this.messageInput.nativeElement.focus();
  }

  sendMessageToGameChat() {
    this._hubService.sendMessage(this.newMessage, ChatDestination.game);
    this.newMessage = '';
  }

  getChatMessageClass(message: ChatMessage) {
    if (message.typeOfMessage === TypeOfMessage.server) {
      return 'server-chat-message';
    } else if (message.typeOfMessage === TypeOfMessage.spectators) {
      return 'spectators-chat-message';
    }
  }

  getChatMessageHidden(message: ChatMessage) {
    if (message.typeOfMessage === TypeOfMessage.spectators && this.sidebarSettings.muteSpectators) {
      return true;
    } else if (message.typeOfMessage === TypeOfMessage.server && this.sidebarSettings.muteServer) {
      return true;
    }
    return false;
  }

  ngOnDestroy(): void {
    this.newMessage += event;
    this._isAlive = false;
  }
}
