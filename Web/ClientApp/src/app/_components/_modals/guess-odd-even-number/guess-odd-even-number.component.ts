import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-guess-odd-even-number',
  templateUrl: './guess-odd-even-number.component.html',
  styleUrls: ['./guess-odd-even-number.component.css']
})
export class GuessOddEvenNumberComponent implements OnInit {

  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  guessOddOrEven(guess: string) {
    this._activeModal.close(guess);
  }

  closeModal(){
    this._activeModal.dismiss();
  }
}
