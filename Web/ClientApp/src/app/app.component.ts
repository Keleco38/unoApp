import { UtilityService } from 'src/app/_services/utility.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(
    private _utilityService: UtilityService,
  ) {}

  ngOnInit(): void {
    if (this._utilityService.userSettings.useDarkTheme) {
      this._utilityService.updateTheme();
    }
  }
}
