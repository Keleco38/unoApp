import { UtilityService } from 'src/app/_services/utility.service';
import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ModalService } from './_services/modal.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  modalOpen = false;

  constructor(private _utilityService: UtilityService, private _router: Router, private _modalService: ModalService) {}

  ngOnInit(): void {
    if (this._utilityService.userSettings.useDarkTheme) {
      this._utilityService.updateTheme();
    }
    var isFirstTimeLunched = this._utilityService.isFirstTimeLunched;

    if (isFirstTimeLunched) {
      this.modalOpen = true;
      var firstTimeLaunchedModal = this._modalService.displayFirstTimeLaunchedModal();
      firstTimeLaunchedModal.result.then((sendToHelpPage: boolean) => {
        this._utilityService.updateFirstTimeLunched();
        this.modalOpen = false;
        if (sendToHelpPage) {
          this._router.navigateByUrl('/help');
        }
      });
    }
  }
}
