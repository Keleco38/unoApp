import { TournamentSetupComponent } from './../_modals/tournament-setup/tournament-setup.component';
import { GameSetupComponent } from './../_modals/game-setup/game-setup.component';
import { Component, OnInit } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { Subscription } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GameType } from 'src/app/_models/enums';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  private _subscription: Subscription;
  userReconnected: boolean;

  constructor(private _hubService: HubService, private _modalService: NgbModal) {}

  ngOnInit() {
    this._subscription = this._hubService.onReconnect.subscribe(() => {
      this.userReconnected = true;
    });
  }

  removeAlert() {
    this.userReconnected = false;
  }

  createGame() {
    this._modalService.open(GameSetupComponent);
  }

  createTournament() {
    this._modalService.open(TournamentSetupComponent);
  }

  ngOnDestroy(): void {
    this._subscription.unsubscribe();
  }
}
