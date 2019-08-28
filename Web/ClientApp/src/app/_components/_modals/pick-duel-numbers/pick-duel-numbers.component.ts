import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-pick-duel-numbers',
  templateUrl: './pick-duel-numbers.component.html',
  styleUrls: ['./pick-duel-numbers.component.css']
})
export class PickDuelNumbersComponent implements OnInit {
  duelNumbers: number[] = [];
  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  closeModal() {
    this._activeModal.dismiss();
  }

  selectOrUnselect(number: number) {
    var index = this.duelNumbers.indexOf(number);
    if (index === -1) {
      if (this.duelNumbers.length == 3) {
        return;
      }
      this.duelNumbers.push(number);
    } else {
      this.duelNumbers.splice(index, 1);
    }
  }
  getBtnClass(number: number) {
    var index = this.duelNumbers.indexOf(number);
    if (index == -1) {
      return 'btn-primary';
    } else {
      return 'btn-success';
    }
  }
  confirm() {
    this._activeModal.close(this.duelNumbers);
  }
}
