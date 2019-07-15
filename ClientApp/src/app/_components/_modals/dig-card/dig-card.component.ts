import { HubService } from './../../../_services/hub.service';
import { Component, OnInit, Input } from '@angular/core';
import { Card } from 'src/app/_models/card';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-dig-card',
  templateUrl: './dig-card.component.html',
  styleUrls: ['./dig-card.component.css']
})
export class DigCardComponent implements OnInit {
  @Input() 'cards': Card[];
  constructor(private _hubService: HubService, private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  digCardFromDiscardedPile(card: Card) {
    this._hubService.digCardFromDiscardedPile(card);
    this._activeModal.dismiss();
  }
}
