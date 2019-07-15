import { Hand } from './../../../_models/hand';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-show-hand',
  templateUrl: './show-hand.component.html',
  styleUrls: ['./show-hand.component.css']
})
export class ShowHandComponent implements OnInit {
  @Input() hand: Hand;
  constructor() {}

  ngOnInit() {}
}
