import { Component, OnInit } from '@angular/core';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';
import { TypeOfMessage } from 'src/app/_models/enums';
import { Game } from 'src/app/_models/game';

@Component({
  selector: 'app-game-chat',
  templateUrl: './game-chat.component.html',
  styleUrls: ['./game-chat.component.css']
})
export class GameChatComponent implements OnInit {
  hideSpectatorsChat = false;
  hideServerChat = false;
  messages: ChatMessage[];
  currentUser: User;
  newMessage = '';
  activeGame: Game;

  constructor(private _hubService: HubService) {}

  ngOnInit(): void {
    this._hubService.gameChatMessages.subscribe(messages => {
      this.messages = messages;
    });
    this._hubService.currentUser.subscribe(user => {
      this.currentUser = user;
    });
    this._hubService.activeGame.subscribe(game => {
      this.activeGame = game;
    });
  }

  sendMessageToGameChat() {
    if (!this.newMessage) {
      return;
    }
    const isSpectator = this.activeGame.spectators.find(spectator => {
      return spectator.user.name === this.currentUser.name;
    });
    if (isSpectator != null) {
      this._hubService.sendMessageToGameChat(this.newMessage, TypeOfMessage.spectators);
    } else {
      this._hubService.sendMessageToGameChat(this.newMessage, TypeOfMessage.chat);
    }

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
    if (message.typeOfMessage === TypeOfMessage.spectators && this.hideSpectatorsChat) {
      return true;
    } else if (message.typeOfMessage === TypeOfMessage.server && this.hideServerChat) {
      return true;
    }
    return false;
  }
}
