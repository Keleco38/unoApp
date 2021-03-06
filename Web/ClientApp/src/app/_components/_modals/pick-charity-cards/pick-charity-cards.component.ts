import { CardValue } from './../../../_models/enums';
import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Card } from 'src/app/_models/card';

@Component({
  selector: 'app-pick-charity-cards',
  templateUrl: './pick-charity-cards.component.html',
  styleUrls: ['./pick-charity-cards.component.css']
})
export class PickCharityCardsComponent implements OnInit {
  @Input() cards: Card[];

  selectedCards: Card[] = [];

  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  selectOrUnselect(card: Card) {
    var index = this.selectedCards.indexOf(card);
    if (index === -1) {
      if (this.selectedCards.length == 2) {
        return;
      }
      this.selectedCards.push(card);
    } else {
      this.selectedCards.splice(index, 1);
    }
  }

  getCardOpacity(card: Card) {
    var index = this.selectedCards.indexOf(card);
    if (index == -1) {
      return '0.25';
    } else {
      return '1';
    }
  }

  closeModal() {
    this._activeModal.dismiss();
  }
  confirm() {
    this._activeModal.close(this.selectedCards.map(card=>card.id));
  }
}
