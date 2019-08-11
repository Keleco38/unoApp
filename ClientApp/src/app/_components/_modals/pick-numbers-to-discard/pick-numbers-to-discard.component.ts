import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-pick-numbers-to-discard',
  templateUrl: './pick-numbers-to-discard.component.html',
  styleUrls: ['./pick-numbers-to-discard.component.css']
})
export class PickNumbersToDiscardComponent implements OnInit {
  numbersToDiscard: number[] = [];
  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  closeModal() {
    this._activeModal.dismiss();
  }

  selectOrUnselect(number: number) {
    var index = this.numbersToDiscard.indexOf(number);
    if (index === -1) {
      if (this.numbersToDiscard.length == 3) {
        return;
      }
      this.numbersToDiscard.push(number);
    } else {
      this.numbersToDiscard.splice(index, 1);
    }
  }
  getBtnClass(number: number) {
    var index = this.numbersToDiscard.indexOf(number);
    if (index == -1) {
      return 'btn-primary';
    } else {
      return 'btn-success';
    }
  }
  confirm() {
    this._activeModal.close(this.numbersToDiscard);
  }
}
