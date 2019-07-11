import { Component, OnInit } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-online-players',
  templateUrl: './online-players.component.html',
  styleUrls: ['./online-players.component.css']
})
export class OnlinePlayersComponent implements OnInit {

  onlineUsers: User[] = new Array<User>();

  constructor(private _hubService: HubService) {}

  ngOnInit(): void {
    this._hubService.onlineUsers.subscribe((onlineUsers: User[]) => {
      this.onlineUsers = onlineUsers;
    });
  }

  buzzUser(user: User) {
    this._hubService.sendMessageToAllChat(`/buzz ${user.name}`);
  }

}
