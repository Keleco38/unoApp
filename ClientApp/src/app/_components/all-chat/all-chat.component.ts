import { Component, OnInit, OnDestroy } from '@angular/core';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';
import { TypeOfMessage } from 'src/app/_models/enums';
import { takeWhile, map, pluck } from 'rxjs/operators';

@Component({
  selector: 'app-all-chat',
  templateUrl: './all-chat.component.html',
  styleUrls: ['./all-chat.component.css']
})
export class AllChatComponent implements OnInit, OnDestroy {
  onlineUsers: string[] = [];

  private _isAlive: boolean = true;
  messages: ChatMessage[];
  currentUser: User;
  newMessage = '';

  constructor(private _hubService: HubService) {}
  ngOnInit(): void {
    this._hubService.allChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(messages => {
      this.messages = messages;
    });
    this._hubService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });
    this._hubService.onlineUsers.pipe(takeWhile(() => this._isAlive)).pipe(
      map(users => {
        return users.map(user => {
          return user.name;
        });
      })
    ).subscribe((userNames:string[])=>{
      this.onlineUsers=userNames;
    });
  }

  sendMessageAllChat() {
    if (!this.newMessage) {
      return;
    }
    this._hubService.sendMessageToAllChat(this.newMessage, false);
    this.newMessage = '';
  }

  getChatMessageClass(message: ChatMessage) {
    if (message.typeOfMessage === TypeOfMessage.server) {
      return 'server-chat-message';
    }
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
