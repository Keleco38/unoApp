import { Component, OnInit } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  private _subscription: Subscription;
  userReconnected: boolean;

  constructor(private _hubService: HubService) {}

  ngOnInit() {
    this._subscription = this._hubService.onReconnect.subscribe(() => {
      this.userReconnected = true;
    });
  }

  removeAlert() {
    this.userReconnected=false;
  }

  createGame() {
    this._hubService.createGame();
  }

  ngOnDestroy(): void {
    this._subscription.unsubscribe();
  }
}
