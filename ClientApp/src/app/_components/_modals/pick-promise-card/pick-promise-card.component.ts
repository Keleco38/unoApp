import { Component, OnInit, Input } from '@angular/core';
import { Card } from 'src/app/_models/card';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-pick-promise-card',
  templateUrl: './pick-promise-card.component.html',
  styleUrls: ['./pick-promise-card.component.css']
})
export class PickPromiseCardComponent implements OnInit {
  @Input() cards: Card[];

  promisedCard: Card = null;

  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  selectOrUnselect(card: Card) {
    if (this.promisedCard != card) {
      if (this.promisedCard != null) {
        return;
      }
      this.promisedCard = card;
    } else {
      this.promisedCard = null;
    }
  }

  getCardOpacity(card: Card) {
    if (this.promisedCard!=card) {
      return '0.25';
    } else {
      return '1';
    }
  }

  closeModal() {
    this._activeModal.dismiss();
  }
  confirm() {
    this._activeModal.close(this.promisedCard.id);
  }
}
