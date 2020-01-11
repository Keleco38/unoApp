import { TournamentStorageService } from './../../_services/storage-services/tournament-storage.service';
import { GameStorageService } from 'src/app/_services/storage-services/game-storage.service';
import { Component, OnInit, OnDestroy, Input, ViewChild, ElementRef } from '@angular/core';
import { UserSettings } from 'src/app/_models/userSettings';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';
import { ModalService } from 'src/app/_services/modal.service';
import { LobbyStorageService } from 'src/app/_services/storage-services/lobby-storage.service';
import { UserStorageService } from 'src/app/_services/storage-services/user-storage.service';
import { UtilityService } from 'src/app/_services/utility.service';
import { takeWhile, map } from 'rxjs/operators';
import { ChatDestination, TypeOfMessage } from 'src/app/_models/enums';
import { KeyValue } from '@angular/common';
import { Card } from 'src/app/_models/card';
import { SidebarSettings } from 'src/app/_models/sidebarSettings';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy {
  @Input('sidebarChatHeight') sidebarChatHeight: number;
  @Input('chatDestination') chatDestination: ChatDestination;
  @Input('emojiPlacement') emojiPlacement: string = 'left';
  @Input('emojiContainer') emojiContainer: string = '';
  @ViewChild('messageInput', { static: false }) messageInput: ElementRef;

  private _isAlive: boolean = true;
  private _myHand: Card[] = [];

  userSettings: UserSettings;
  onlineUsers: string[] = [];
  messages: ChatMessage[];
  currentUser: User;
  newMessage = '';
  sidebarSettings: SidebarSettings;

  constructor(
    private _hubService: HubService,
    private _modalService: ModalService,
    private _lobbyStorageService: LobbyStorageService,
    private _userStorageService: UserStorageService,
    private _gameStorageService: GameStorageService,
    private _tournamentStorageService: TournamentStorageService,
    private _utilityService: UtilityService
  ) {}
  ngOnInit(): void {
    this.userSettings = this._utilityService.userSettings;
    this.sidebarSettings = this._utilityService.sidebarSettings;

    if (this.chatDestination == ChatDestination.all) {
      this._lobbyStorageService.allChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(messages => {
        this.messages = messages;
      });
    } else if (this.chatDestination == ChatDestination.tournament) {
      this._tournamentStorageService.tournamentChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(messages => {
        this.messages = messages;
      });
    } else {
      this._gameStorageService.gameChat.pipe(takeWhile(() => this._isAlive)).subscribe(messages => {
        this.messages = messages;
      });
    }

    this._gameStorageService.myHand.pipe(takeWhile(() => this._isAlive)).subscribe(cards => {
      this._myHand = cards;
    });

    this._hubService.updateGameEnded.pipe(takeWhile(() => this._isAlive)).subscribe(gameEndedResult => {
      this.messageInput.nativeElement.blur();
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

  sendMessage(event) {
    if (!this.newMessage) {
      return;
    }
    if (this.newMessage == '/admin') {
      this.newMessage = '';
      event.target.children[0].blur();
      this._modalService.displayAdminSectionModal();
      return;
    }

    if (this.newMessage == '/hand' && this._myHand.length > 0) {
      this.newMessage = '';
      event.target.children[0].blur();
      var cardsAndUser: KeyValue<string, Card[]>[] = [{ key: 'My Cards', value: this._myHand }];
      this._modalService.displayShowCardsModal(cardsAndUser, true);
      return;
    }

    this._hubService.sendMessage(this.newMessage, this.chatDestination);
    this.newMessage = '';
  }


  getChatMessageClass(message: ChatMessage) {
    if (message.typeOfMessage === TypeOfMessage.server) {
      return 'server-chat-message';
    } else if (message.typeOfMessage === TypeOfMessage.spectators) {
      return 'spectators-chat-message';
    }
  }

  addEmojiToChat(event) {
    this.newMessage += event;
    this.messageInput.nativeElement.focus();
  }

  getChatMessageHidden(message: ChatMessage) {
    if (this.chatDestination == ChatDestination.all) return false;
    if (message.typeOfMessage === TypeOfMessage.spectators && this.sidebarSettings.muteSpectators) {
      return true;
    } else if (message.typeOfMessage === TypeOfMessage.server && this.sidebarSettings.muteServer) {
      return true;
    }
    return false;
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
