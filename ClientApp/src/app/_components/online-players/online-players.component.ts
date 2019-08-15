import { Component, OnInit, OnDestroy } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { User } from 'src/app/_models/user';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-online-players',
  templateUrl: './online-players.component.html',
  styleUrls: ['./online-players.component.css']
})
export class OnlinePlayersComponent implements OnInit, OnDestroy {
  private _isAlive: boolean = true;
  onlineUsers: User[] = new Array<User>();

  constructor(private _hubService: HubService) {}

  ngOnInit(): void {
    this._hubService.onlineUsers.pipe(takeWhile(() => this._isAlive)).subscribe((onlineUsers: User[]) => {
      this.onlineUsers = onlineUsers;
    });
  }

  buzzUser(user: User, buzzType: string) {
    this._hubService.sendMessageToAllChat(`/${buzzType} ${user.name}`);
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }

}
