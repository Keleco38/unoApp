import { TournamentStorageService } from './../../_services/storage-services/tournament-storage.service';
import { HubService } from 'src/app/_services/hub.service';
import { Tournament } from './../../_models/tournament';
import { User } from 'src/app/_models/user';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { Component, OnInit, Input } from '@angular/core';
import { SidebarSettings } from '../../_models/sidebarSettings';
import { UtilityService } from '../../_services/utility.service';
import { takeWhile, map } from 'rxjs/operators';
import { TypeOfMessage, ChatDestination } from '../../_models/enums';
import { LobbyStorageService } from 'src/app/_services/storage-services/lobby-storage.service';
import { UserStorageService } from 'src/app/_services/storage-services/user-storage.service';
import { GameStorageService } from 'src/app/_services/storage-services/game-storage.service';

@Component({
  selector: 'app-tournament-chat',
  templateUrl: './tournament-chat.component.html',
  styleUrls: ['./tournament-chat.component.css']
})
export class TournamentChatComponent implements OnInit {
  @Input('heightClassString') heightClassString: string;

  private _isAlive: boolean = true;
  onlineUsers: string[];
  messages: ChatMessage[];
  currentUser: User;
  newMessage = '';
  activeTournament: Tournament;
  sidebarSettings: SidebarSettings;

  constructor(
    private _hubService: HubService,
    private _userStorageService: UserStorageService,
    private _utilityService: UtilityService,
    private _lobbyStorageService: LobbyStorageService,
    private _gameStorageService: GameStorageService,
    private _tournamentStorageService: TournamentStorageService
  ) {}

  ngOnInit(): void {
    this._tournamentStorageService.tournamentChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(messages => {
      this.messages = messages;
    });
    this._userStorageService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });
    this._tournamentStorageService.activeTournament.pipe(takeWhile(() => this._isAlive)).subscribe(activeTournament => {
      this.activeTournament = activeTournament;
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

  sendMessageToTournamentChat() {
    this._hubService.sendMessage(this.newMessage, ChatDestination.tournament);
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
    this._isAlive = false;
  }
}
