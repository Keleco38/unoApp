import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { HubService } from 'src/app/_services/hub.service';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-input-password',
  templateUrl: './input-password.component.html',
  styleUrls: ['./input-password.component.css']
})
export class InputPasswordComponent implements OnInit, OnDestroy {
  @Input() isTournament: boolean;
  @Input() id: string;
  @Input() name: string;
  @Input() started: boolean;
  @Input() numberOfPlayers: number;
  @Input() maxNumberOfPlayers: number;

  private _isAlive = false;
  password = '';

  constructor(private _hubService: HubService, private _activeModal:NgbActiveModal) {}

  ngOnInit() {}

  confirm() {
    if (this.isTournament) {
      this._hubService.joinTournament(this.id, this.password);
    } else {
      this._hubService.joinGame(this.id, this.password);
    }
    this.closeModal();
  }

  closeModal(){
    this._activeModal.dismiss();
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
