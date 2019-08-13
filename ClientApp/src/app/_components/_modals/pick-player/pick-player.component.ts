import { Component, OnInit, Input } from '@angular/core';
import { Player } from 'src/app/_models/player';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-pick-player',
  templateUrl: './pick-player.component.html',
  styleUrls: ['./pick-player.component.css']
})
export class PickPlayerComponent implements OnInit {
  @Input() players: Player[];
  @Input() currentUser: User;
  constructor(public activeModal: NgbActiveModal) {}

  ngOnInit() {
  }
  selectPlayer(player: Player) {
    this.activeModal.close(player.id);
  }
}
