import { Component, OnInit, Input } from "@angular/core";
import { CardValue } from "src/app/_models/enums";
import { KeyValue } from "@angular/common";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { UtilityService } from "src/app/_services/utility.service";

@Component({
  selector: "app-pick-wild-card",
  templateUrl: "./pick-wild-card.component.html",
  styleUrls: ["./pick-wild-card.component.css"]
})
export class PickWildCardComponent implements OnInit {
  @Input() bannedCards: CardValue[];
  selectedCardToReturn: CardValue = null;
  allWildCards: KeyValue<string, number>[];

  constructor(
    private _activeModal: NgbActiveModal,
    private _utilityService: UtilityService
  ) {}

  ngOnInit() {
    var allCards = this._utilityService.allCards;
    this.allWildCards=allCards.filter(x=>x.value>12 && x.value!=17 && this.bannedCards.indexOf(x.value)==-1);
  }

  closeModal() {
    this._activeModal.dismiss();
  }

  getBannedCardName(bannedCard: CardValue) {
    return this.allWildCards.find(c => c.value == bannedCard).key;
  }

  selectOrUnselect(pickedCardValue: CardValue) {
    this.selectedCardToReturn = pickedCardValue;
  }
  getBtnClass(pickedCardValue: CardValue) {
    if (this.selectedCardToReturn != pickedCardValue) {
      return "btn-primary";
    } else {
      return "btn-success";
    }
  }

  confirm() {
    if (this.selectedCardToReturn == null) return;
    this._activeModal.close(this.selectedCardToReturn);
  }
}
