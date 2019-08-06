import { HubService } from './../../../_services/hub.service';
import { Component, OnInit, Input, Injector } from '@angular/core';
import { Card } from 'src/app/_models/card';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-dig-card',
  templateUrl: './dig-card.component.html',
  styleUrls: ['./dig-card.component.css']
})
export class DigCardComponent implements OnInit {
  @Input() 'discardedPile': Card[];
  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  digCardFromDiscardedPile(cardToDig: Card) {
    this._activeModal.close(cardToDig);
  }
}
