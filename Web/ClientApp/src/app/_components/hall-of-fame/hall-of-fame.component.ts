import { HallOfFame } from './../../_models/hallOfFame';
import { HttpService } from './../../_services/http.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-hall-of-fame',
  templateUrl: './hall-of-fame.component.html',
  styleUrls: ['./hall-of-fame.component.css']
})
export class HallOfFameComponent implements OnInit {
  hallOfFameStats: HallOfFame[] = [];

  constructor(private _httpService: HttpService) {}

  ngOnInit() {
    this._httpService.getHallOfFameStats().subscribe((hallOfFameStats: HallOfFame[]) => {
      this.hallOfFameStats = hallOfFameStats;
    });
  }
}
