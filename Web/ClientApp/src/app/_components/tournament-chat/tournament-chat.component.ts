import { HubService } from 'src/app/_services/hub.service';
import { Tournament } from './../../_models/tournament';
import { User } from 'src/app/_models/user';
import { ChatMessage } from 'src/app/_models/chatMessage';
import { Component, OnInit } from '@angular/core';
import { SidebarSettings } from '../../_models/sidebarSettings';
import { UtilityService } from '../../_services/utility.service';
import { takeWhile, map } from 'rxjs/operators';
import { TypeOfMessage, ChatDestination } from '../../_models/enums';

@Component({
  selector: 'app-tournament-chat',
  templateUrl: './tournament-chat.component.html',
  styleUrls: ['./tournament-chat.component.css']
})
export class TournamentChatComponent implements OnInit {

  private _isAlive: boolean = true;
  onlineUsers: string[];
  messages: ChatMessage[];
  currentUser: User;
  newMessage = '';
  activeTournament: Tournament;
  sidebarSettings: SidebarSettings;

  constructor(private _hubService: HubService, private _utilityService: UtilityService) {}

  ngOnInit(): void {
    this._hubService.tournamentChatMessages.pipe(takeWhile(() => this._isAlive)).subscribe(messages => {
      this.messages = messages;
    });
    this._hubService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });
    this._hubService.activeTournament.pipe(takeWhile(() => this._isAlive)).subscribe(activeTournament => {
      this.activeTournament = activeTournament;
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
