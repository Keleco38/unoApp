import { CardValue } from './../../../_models/enums';
import { Component, OnInit, Input } from '@angular/core';
import { Hand } from 'src/app/_models/hand';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Card } from 'src/app/_models/card';

@Component({
  selector: 'app-pick-charity-cards',
  templateUrl: './pick-charity-cards.component.html',
  styleUrls: ['./pick-charity-cards.component.css']
})
export class PickCharityCardsComponent implements OnInit {
  @Input() hand: Hand;

  selectedCards: Card[] = [];

  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  selectOrUnselect(card: Card) {
    var index = this.selectedCards.indexOf(card);
    if (index === -1) {
      if (this.selectedCards.length == 3) {
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
  isCharityCard(card: Card) {
    return card.value == CardValue.charity;
  }

  closeModal() {
    this._activeModal.dismiss();
  }
  confirm() {
    this._activeModal.close(this.selectedCards);
  }
}
