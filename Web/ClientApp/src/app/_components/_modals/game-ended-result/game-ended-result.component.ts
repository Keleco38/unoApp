import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, OnInit, Input } from '@angular/core';
import { GameEndedResult } from 'src/app/_models/gameEndedResult';

@Component({
  selector: 'app-game-ended-result',
  templateUrl: './game-ended-result.component.html',
  styleUrls: ['./game-ended-result.component.css']
})
export class GameEndedResultComponent implements OnInit {
  @Input() gameEndedResult: GameEndedResult;

  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  closeModal() {
    this._activeModal.dismiss();
  }
}
