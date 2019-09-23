import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { User } from 'src/app/_models/user';
import { HubService } from './../../../_services/hub.service';
import { UserSettings } from './../../../_models/userSettings';
import { UtilityService } from 'src/app/_services/utility.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrls: ['./user-settings.component.css']
})
export class UserSettingsComponent implements OnInit, OnDestroy {
  private _subscription: Subscription;
  userSettings: UserSettings;
  currentUser: User;

  constructor(private _hubService: HubService, private _utilityService: UtilityService, private _activeModal: NgbActiveModal) {}

  ngOnInit() {
    this.userSettings = this._utilityService.userSettings;
    this._subscription = this._hubService.currentUser.subscribe((currentUser: User) => {
      this.currentUser = currentUser;
    });
  }

  rename() {
    this._hubService.addOrRenameUser(true);
  }

  updateBlockedBuzzCommands(event, buzzType) {
    var checked=event.target.checked;
    if (checked === true) {
      this.userSettings.blockedBuzzCommands.push(buzzType);
    } else {
      var index = this.userSettings.blockedBuzzCommands.indexOf(buzzType);
      this.userSettings.blockedBuzzCommands.splice(index, 1);
    }
    this.saveUserSettings();
  }

  shouldBeCheckedBuzzCommand(buzzType) {
    return this.userSettings.blockedBuzzCommands.indexOf(buzzType) != -1;
  }

  saveUserSettings() {
    this._utilityService.saveUserSettings();
  }

  closeModal() {
    this._activeModal.close();
  }

  ngOnDestroy(): void {
    this._subscription.unsubscribe();
  }
}
