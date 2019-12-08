import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-activate-special-effect',
  templateUrl: './activate-special-effect.component.html',
  styleUrls: ['./activate-special-effect.component.css']
})
export class ActivateSpecialEffectComponent implements OnInit {
  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  selectResult(result: boolean) {
    this._activeModal.close(result);
  }

  closeModal(){
    this._activeModal.dismiss();
  }
}
