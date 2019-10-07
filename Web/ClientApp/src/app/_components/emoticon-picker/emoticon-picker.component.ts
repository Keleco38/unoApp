import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-emoticon-picker',
  templateUrl: './emoticon-picker.component.html',
  styleUrls: ['./emoticon-picker.component.css']
})
export class EmoticonPickerComponent implements OnInit {
  @Output('emoji') emoji=new EventEmitter();
  constructor() { }

  ngOnInit() {
  }

  addEmoji(emoji){
    this.emoji.emit(emoji);
  }

}
