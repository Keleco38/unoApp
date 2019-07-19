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
  @Input() 'cards': Card[];
  private _hubService: HubService;
  constructor(private _injector: Injector, private _activeModal: NgbActiveModal) {
    this._hubService = this._injector.get(HubService);
  }

  ngOnInit() {}

  digCardFromDiscardedPile(card: Card) {
    this._hubService.digCardFromDiscardedPile(card);
    this._activeModal.dismiss();
  }
}
