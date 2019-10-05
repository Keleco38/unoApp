import { UserStorageService } from './../../_services/storage-services/user-storage.service';
import { LobbyStorageService } from './../../_services/storage-services/lobby-storage.service';
import { ModalService } from '../../_services/modal.service';
import { Component, OnInit, OnDestroy, Input, ViewChild, ElementRef } from '@angular/core';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';
import { TypeOfMessage, ChatDestination } from 'src/app/_models/enums';
import { takeWhile, map, pluck } from 'rxjs/operators';

@Component({
  selector: 'app-all-chat',
  templateUrl: './all-chat.component.html',
  styleUrls: ['./all-chat.component.css']
})
export class AllChatComponent implements OnInit, OnDestroy {
  @Input('heightClassString') heightClassString: string;
  @ViewChild('messageInput', { static: false }) messageInput: ElementRef;

  private _isAlive: boolean = true;
  onlineUsers: string[] = [];
  messages: ChatMessage[];
  currentUser: User;
  newMessage = '';

  constructor(
    private _hubService: HubService,
    private _modalService: ModalService,
    private _lobbyStorageService: LobbyStorageService,
    private _userStorageService: UserStorageService
  ) {}
  ngOnInit(): void {
    this._lobbyStorageService.allChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(messages => {
      this.messages = messages;
    });
    this._userStorageService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
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
  }

  sendMessageAllChat(event) {
    if (!this.newMessage) {
      return;
    }
    if (this.newMessage == '/admin') {
      this.newMessage = '';
      event.target.children[0].blur();
      this._modalService.displayAdminSectionModal();
      return;
    }

    if (this.newMessage == '/tournament') {
      this.newMessage = '';
      event.target.children[0].blur();
      this._modalService.displayTournamentSetupModal();
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

  addEmojiToChat(event) {
    this.newMessage += event;
    this.messageInput.nativeElement.focus();
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
