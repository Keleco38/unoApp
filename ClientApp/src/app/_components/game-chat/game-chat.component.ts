import { Component, OnInit } from '@angular/core';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';
import { TypeOfMessage } from 'src/app/_models/enums';

@Component({
  selector: 'app-game-chat',
  templateUrl: './game-chat.component.html',
  styleUrls: ['./game-chat.component.css']
})
export class GameChatComponent implements OnInit {
  hideSpectatorsChat = false;
  messages: ChatMessage[];
  currentUser: User;
  newMessage = '';

  constructor(private _hubService: HubService) {}

  ngOnInit(): void {
    this._hubService.gameChatMessages.subscribe(messages => {
      this.messages=messages;
    });
    this._hubService.currentUser.subscribe(user => {
      this.currentUser = user;
    });
  }

  sendMessageToGameChat() {
    if (!this.newMessage) {
      return;
    }
    this._hubService.sendMessageToGameChat(this.newMessage);
    this.newMessage = '';
  }

  getChatMessageClass(message: ChatMessage) {
    if (message.typeOfMessage === TypeOfMessage.server) {
      return 'server-chat-message';
    }
  }

  getChatMessageHidden(message: ChatMessage) {
    return message.typeOfMessage === TypeOfMessage.spectators && this.hideSpectatorsChat;
  }
}
