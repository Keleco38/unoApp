import { ModalService } from '../../_services/modal.service';
import { TournamentSetupComponent } from './../_modals/tournament-setup/tournament-setup.component';
import { GameSetupComponent } from './../_modals/game-setup/game-setup.component';
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

  constructor(private _hubService: HubService, private _modalService: ModalService) {}

  ngOnInit() {
    this._subscription = this._hubService.updateOnReconnect.subscribe(() => {
      this.userReconnected = true;
    });
  }

  removeAlert() {
    this.userReconnected = false;
  }

  createGame() {
    this._modalService.displayGameSetupModal();
  }

  ngOnDestroy(): void {
    this._subscription.unsubscribe();
  }
}
