import { CardColor } from 'src/app/_models/enums';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-pick-color',
  templateUrl: './pick-color.component.html',
  styleUrls: ['./pick-color.component.css']
})
export class PickColorComponent implements OnInit {
  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  pickColor(cardColor: CardColor) {
    this._activeModal.close(cardColor);
  }
  closeModal(){
    this._activeModal.dismiss();
  }
}
