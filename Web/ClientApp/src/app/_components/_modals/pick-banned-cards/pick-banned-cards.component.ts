import { UtilityService } from './../../../_services/utility.service';
import { CardValue } from './../../../_models/enums';
import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { KeyValue } from '@angular/common';

@Component({
  selector: 'app-pick-banned-cards',
  templateUrl: './pick-banned-cards.component.html',
  styleUrls: ['./pick-banned-cards.component.css']
})
export class PickBannedCardsComponent implements OnInit {
  @Input() bannedCards: CardValue[];
  allCards: KeyValue<string, number>[];

  constructor(private _activeModal: NgbActiveModal, private _utilityService: UtilityService) {}

  ngOnInit() {
    this.allCards = this._utilityService.allCards;
  }

  closeModal() {
    this._activeModal.dismiss();
  }

  getBannedCardName(bannedCard: CardValue) {
    return this.allCards.find(c => c.value == bannedCard).key;
  }

  selectOrUnselect(bannedCard: CardValue) {
    var index = this.bannedCards.indexOf(bannedCard);
    if (index === -1) {
      if (this.bannedCards.length == 10) {
        return;
      }
      this.bannedCards.push(bannedCard);
    } else {
      this.bannedCards.splice(index, 1);
    }
  }
  getBtnClass(bannedCard: CardValue) {
    var index = this.bannedCards.indexOf(bannedCard);
    if (index == -1) {
      return 'btn-primary';
    } else {
      return 'btn-success';
    }
  }
  confirm() {
    this._activeModal.close(this.bannedCards);
  }
}
