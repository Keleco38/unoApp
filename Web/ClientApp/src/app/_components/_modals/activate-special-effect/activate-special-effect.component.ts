import { UtilityService } from './../../../_services/utility.service';
import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UserSettings } from 'src/app/_models/userSettings';

@Component({
  selector: 'app-activate-special-effect',
  templateUrl: './activate-special-effect.component.html',
  styleUrls: ['./activate-special-effect.component.css']
})
export class ActivateSpecialEffectComponent implements OnInit {
  @Input() description: string;
  userSettings: UserSettings;
  constructor(private _activeModal: NgbActiveModal, private _utilityService: UtilityService) { }

  ngOnInit() {
    this.userSettings = this._utilityService.userSettings;
  }

  selectResult(result: boolean) {
    this._activeModal.close(result);
  }

  closeModal() {
    this._activeModal.dismiss();
  }
}
