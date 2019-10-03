import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';

@Component({
  selector: 'app-kick-ban-player',
  templateUrl: './kick-ban-player.component.html',
  styleUrls: ['./kick-ban-player.component.css']
})
export class KickBanPlayerComponent implements OnInit, OnDestroy {
  @Input('isTournament') isTournament: boolean;
  @Input('userToKick') userToKick: User;

  private _isAlive = true;
  hasPicked = false;
  isBan = false;

  constructor(private _hubService: HubService, private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  closeModal() {
    this._activeModal.dismiss();
  }

  confirmAction() {
    if (this.isTournament) {
      this._hubService.kickContestantFromTournament(this.isBan, this.userToKick.name);
    } else {
      this._hubService.kickPlayerFromGame(this.isBan, this.userToKick.name);
    }
    this.closeModal();
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
