import { HubService } from './_services/hub.service';
import { UtilityService } from 'src/app/_services/utility.service';
import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ModalService } from './_services/modal.service';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  private _isAlive = true;

  modalOpen = false;
  userReconnected: boolean;

  constructor(
    private _utilityService: UtilityService,
    private _router: Router,
    private _modalService: ModalService,
    private _hubService: HubService
  ) {}

  ngOnInit(): void {
    this._hubService.updateOnReconnect.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this.userReconnected = true;
    });

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

  removeAlert() {
    this.userReconnected = false;
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
