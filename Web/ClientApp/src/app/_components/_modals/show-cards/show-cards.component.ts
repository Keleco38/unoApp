import { UtilityService } from 'src/app/_services/utility.service';
import { KeyValue } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, OnInit, Input } from '@angular/core';
import { Card } from 'src/app/_models/card';
import { CardValue } from 'src/app/_models/enums';
import { UserSettings } from 'src/app/_models/userSettings';

@Component({
  selector: 'app-show-cards',
  templateUrl: './show-cards.component.html',
  styleUrls: ['./show-cards.component.css']
})
export class ShowCardsComponent implements OnInit {
  @Input() cardsAndNames: KeyValue<string, Card[]>[];
  @Input() detailed: boolean;
  private allCards: any;
  userSettings: UserSettings;
  constructor(private _activeModal: NgbActiveModal, private _utilityService:UtilityService) {}

  ngOnInit() {
    this.allCards = this._utilityService.allCards;
    this.userSettings=this._utilityService.userSettings;
  }

  closeModal() {
    this._activeModal.close();
  }

  getCardName(card: CardValue) {
    return this.allCards.find(c => c.value == card).key;
  }
}
