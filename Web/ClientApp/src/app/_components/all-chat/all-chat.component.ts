import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';
import { TypeOfMessage, ChatDestination } from 'src/app/_models/enums';
import { takeWhile, map, pluck } from 'rxjs/operators';
import { AdminSectionComponent } from '../_modals/admin-section/admin-section.component';

@Component({
  selector: 'app-all-chat',
  templateUrl: './all-chat.component.html',
  styleUrls: ['./all-chat.component.css']
})
export class AllChatComponent implements OnInit, OnDestroy {
  private _isAlive: boolean = true;
  onlineUsers: string[] = [];
  messages: ChatMessage[];
  currentUser: User;
  newMessage = '';

  constructor(private _hubService: HubService, private _modalService: NgbModal) {}
  ngOnInit(): void {
    this._hubService.allChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(messages => {
      this.messages = messages;
    });
    this._hubService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });
    this._hubService.onlineUsers
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
  }

  sendMessageAllChat(event) {
    if (!this.newMessage) {
      return;
    }
    if (this.newMessage == '/admin') {
      this.newMessage = '';
      event.target.children[0].blur();
      this._modalService.open(AdminSectionComponent);
      return;
    }

    this._hubService.sendMessage(this.newMessage, ChatDestination.all);
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
