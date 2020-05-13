import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Card } from 'src/app/_models/card';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UtilityService } from 'src/app/_services/utility.service';
import { UserSettings } from 'src/app/_models/userSettings';

@Component({
  selector: 'app-DrawAutoPlay',
  templateUrl: './DrawAutoPlay.component.html',
  styleUrls: ['./DrawAutoPlay.component.css']
})
export class DrawAutoPlayComponent implements OnInit,OnDestroy {
  @Input() card: Card;
  @Input() invalidMove: boolean;
  userSettings: UserSettings;

  private _isAlive = true;

  constructor(private _activeModal: NgbActiveModal, private _utilityService: UtilityService) { }

  ngOnInit() {
    this.userSettings = this._utilityService.userSettings;
  }
  confirmAction(result) {
    this._activeModal.close(result);
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }

}
