import { Component, OnInit, Input } from '@angular/core';
import { Card } from 'src/app/_models/card';

@Component({
  selector: 'app-show-hand',
  templateUrl: './show-hand.component.html',
  styleUrls: ['./show-hand.component.css']
})
export class ShowHandComponent implements OnInit {
  @Input() cards: Card[];
  constructor() {}

  ngOnInit() {}
}
