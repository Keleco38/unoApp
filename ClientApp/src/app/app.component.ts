import { HubService } from 'src/app/_services/hub.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  private _subscription: Subscription;
  userReconnected: boolean;

  constructor(private _hubService: HubService) {}

  ngOnInit(): void {
    this._subscription = this._hubService.onReconnect.subscribe(() => {
      this.userReconnected = true;
    });
  }

  remoteAlert() {
    this.userReconnected=false;
  }

  ngOnDestroy(): void {
    this._subscription.unsubscribe();
  }
}
