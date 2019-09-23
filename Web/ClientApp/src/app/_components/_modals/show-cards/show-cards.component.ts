import { KeyValue } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, OnInit, Input } from '@angular/core';
import { Card } from 'src/app/_models/card';

@Component({
  selector: 'app-show-cards',
  templateUrl: './show-cards.component.html',
  styleUrls: ['./show-cards.component.css']
})
export class ShowCardsComponent implements OnInit {
  @Input() cardsAndNames: KeyValue<string, Card[]>[];
  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  closeModal() {
    this._activeModal.close();
  }
}
