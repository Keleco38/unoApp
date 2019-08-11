import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-Blackjack',
  templateUrl: './Blackjack.component.html',
  styleUrls: ['./Blackjack.component.css']
})
export class BlackjackComponent implements OnInit {
  blackjackNumber: number = 0;
  numbersHit: number[] = [];
  lastNumberHit: number=0;
  possibleNumbers: number[] = [];
  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {
    for (let i = 0; i < 4; i++) {
      this.possibleNumbers.push(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10);
    }
  }

  hitMe() {
    this.lastNumberHit = this.possibleNumbers[Math.floor(Math.random() * this.possibleNumbers.length)];
    this.numbersHit.push(this.lastNumberHit);
    this.blackjackNumber += this.lastNumberHit;
    if (this.blackjackNumber >= 21) {
      this.confirm();
    }
  }

  confirm() {
    this._activeModal.close(this.blackjackNumber);
  }
}
