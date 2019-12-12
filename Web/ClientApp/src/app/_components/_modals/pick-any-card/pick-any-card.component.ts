import { Component, OnInit, Input } from "@angular/core";
import { CardValue } from "src/app/_models/enums";
import { KeyValue } from "@angular/common";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { UtilityService } from "src/app/_services/utility.service";

@Component({
  selector: "app-pick-any-card",
  templateUrl: "./pick-any-card.component.html",
  styleUrls: ["./pick-any-card.component.css"]
})
export class PickAnyCardComponent implements OnInit {
  selectedCardToReturn: CardValue = null;

  private _allCards: KeyValue<string, number>[];

  standardCards: KeyValue<string, number>[];
  specialCards: KeyValue<string, number>[];

  constructor(
    private _activeModal: NgbActiveModal,
    private _utilityService: UtilityService
  ) { }

  ngOnInit() {
    this._allCards = this._utilityService.allCards;
    this.standardCards = this._allCards.filter(x => x.value < 15);
    this.specialCards = this._allCards.filter(x => x.value > 14);
  }

  closeModal() {
    this._activeModal.dismiss();
  }

  getBannedCardName(bannedCard: CardValue) {
    return this._allCards.find(c => c.value == bannedCard).key;
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
