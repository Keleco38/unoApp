import { UtilityService } from 'src/app/_services/utility.service';
import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(private _utilityService: UtilityService, private _router: Router) {}

  ngOnInit(): void {
    if (this._utilityService.userSettings.useDarkTheme) {
      this._utilityService.updateTheme();
    }
    var isFirstTimeLunched = this._utilityService.isFirstTimeLunched;
    if (isFirstTimeLunched === true) {
      var navigateToHelpPage = confirm(
        "This is the first time you have launched this game. We suggest that you first read 'Help' page. Do you want us to take you there?"
      );
      if (navigateToHelpPage === true) {
        this._router.navigateByUrl('/help');
      }
      this._utilityService.updateFirstTimeLunched();
    }
  }
}
