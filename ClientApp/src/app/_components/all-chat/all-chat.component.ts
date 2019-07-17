import { Component, OnInit } from '@angular/core';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';
import { TypeOfMessage } from 'src/app/_models/enums';

@Component({
  selector: 'app-all-chat',
  templateUrl: './all-chat.component.html',
  styleUrls: ['./all-chat.component.css']
})
export class AllChatComponent implements OnInit {
  messages: ChatMessage[];
  currentUser: User;
  newMessage = '';

  constructor(private _hubService: HubService) {}
  ngOnInit(): void {
    this._hubService.allChatMessages.subscribe(messages => {
      this.messages = messages;
    });
    this._hubService.currentUser.subscribe(user => {
      this.currentUser = user;
    });
  }

  sendMessageAllChat() {
    if (!this.newMessage) {
      return;
    }
    this._hubService.sendMessageToAllChat(this.newMessage);
    this.newMessage = '';
  }

  getChatMessageClass(message: ChatMessage) {
    if (message.typeOfMessage === TypeOfMessage.server) {
      return 'server-chat-message';
    }
  }
}
