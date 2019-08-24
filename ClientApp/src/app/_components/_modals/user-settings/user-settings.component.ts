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

  updateBlockedBuzzComands(event, buzzType) {
    var value = event.target.checked;
    if (value === true) {
      this.userSettings.blockedBuzzCommands.push(buzzType);
    } else {
      var index = this.userSettings.blockedBuzzCommands.indexOf(buzzType);
      this.userSettings.blockedBuzzCommands.splice(index, 1);
    }
    this._utilityService.saveUserSettings();
  }

  updateNotifyWhenTurnToPlay(event) {
    var value = event.target.checked;
    this.userSettings.notifyUserWhenHisTurnToPlay = value;
    this._utilityService.saveUserSettings();
  }

  updateTextNotifyOnMentions(event) {
    var value = event.target.checked;
    this.userSettings.notifyWhenMentionedToast = value;
    this._utilityService.saveUserSettings();
  }
  updateSoundNotifyOnMentions(event) {
    var value = event.target.checked;
    this.userSettings.notifyWhenMentionedBuzz = value;
    this._utilityService.saveUserSettings();
  }

  shouldBeCheckedBuzzComand(buzzType) {
    return this.userSettings.blockedBuzzCommands.indexOf(buzzType) != -1;
  }

  shouldBeCheckedNotifyWhenTurnToPlay() {
    return this.userSettings.notifyUserWhenHisTurnToPlay;
  }

  shouldBeCheckedTextNotifyOnMentions() {
    return this.userSettings.notifyWhenMentionedToast;
  }

  shouldBeCheckedSoundNotifyOnMentions() {
    return this.userSettings.notifyWhenMentionedBuzz;
  }

  

  closeModal() {
    this._activeModal.close();
  }

  ngOnDestroy(): void {
    this._subscription.unsubscribe();
  }
}
