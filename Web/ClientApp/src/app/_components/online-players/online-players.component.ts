import { LobbyStorageService } from './../../_services/storage-services/lobby-storage.service';
import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { User } from 'src/app/_models/user';
import { takeWhile } from 'rxjs/operators';
import { ChatDestination } from 'src/app/_models/enums';

@Component({
  selector: 'app-online-players',
  templateUrl: './online-players.component.html',
  styleUrls: ['./online-players.component.css']
})
export class OnlinePlayersComponent implements OnInit, OnDestroy {
  @Input('buzzContainer') buzzContainer: string = '-';
  @Input('buzzPlacement') buzzPlacement: string = 'auto';

  private _isAlive: boolean = true;
  onlineUsers: User[] = new Array<User>();

  constructor(private _hubService: HubService, private _lobbyStorageService:LobbyStorageService) {}

  ngOnInit(): void {
    this._lobbyStorageService.onlineUsers.pipe(takeWhile(() => this._isAlive)).subscribe((onlineUsers: User[]) => {
      this.onlineUsers = onlineUsers;
    });
  }

  buzzUser(user: User, buzzType: string) {
    this._hubService.sendMessage(`/${buzzType} ${user.name}`, ChatDestination.all);
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }

}
