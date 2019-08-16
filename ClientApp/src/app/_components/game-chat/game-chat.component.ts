import { SidebarSettings } from './../../_models/sidebarSettings';
import { UtilityService } from './../../_services/utility.service';
import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';
import { TypeOfMessage } from 'src/app/_models/enums';
import { Game } from 'src/app/_models/game';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-game-chat',
  templateUrl: './game-chat.component.html',
  styleUrls: ['./game-chat.component.css']
})
export class GameChatComponent implements OnInit, OnDestroy {
  ngOnDestroy(): void {
    this._isAlive = false;
  }
  private _isAlive: boolean = true;
  messages: ChatMessage[];
  currentUser: User;
  newMessage = '';
  activeGame: Game;
  sidebarSettings: SidebarSettings;

  constructor(private _hubService: HubService, private _utilityService: UtilityService) {}

  ngOnInit(): void {
    this._hubService.gameChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(messages => {
      this.messages = messages;
    });
    this._hubService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });
    this._hubService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      this.activeGame = game;
    });
    this.sidebarSettings = this._utilityService.sidebarSettings;
  }

  sendMessageToGameChat() {
    this._hubService.sendMessageToGameChat(this.newMessage);
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
}
