import { GameMode } from './../../_models/enums';
import { Component, OnInit } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  constructor(private _hubService: HubService) {}

  ngOnInit() {}
  rename() {
    this._hubService.addOrRenameUser(true);
  }

  createGame() {
    this._hubService.createGame();
  }
}
