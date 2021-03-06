import { ModalService } from './../../../_services/modal.service';
import { UserStorageService } from './../../../_services/storage-services/user-storage.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { User } from 'src/app/_models/user';
import { HubService } from './../../../_services/hub.service';
import { UserSettings } from './../../../_models/userSettings';
import { UtilityService } from 'src/app/_services/utility.service';
import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrls: ['./user-settings.component.css']
})
export class UserSettingsComponent implements OnInit, OnDestroy {
  @Output('displayRenameModal') displayRenameModal=new EventEmitter();

  private _subscription: Subscription;

  userSettings: UserSettings;
  currentUser: User;

  constructor(private _userStorageService:UserStorageService, private _utilityService: UtilityService,private _activeModal: NgbActiveModal) {}

  ngOnInit() {
    this.userSettings = this._utilityService.userSettings;
    this._subscription = this._userStorageService.currentUser.subscribe((currentUser: User) => {
      this.currentUser = currentUser;
    });
  }

  rename() {
    this.displayRenameModal.emit();
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

  updateUseDarkTheme(){
    this.saveUserSettings();
    this._utilityService.updateTheme();
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
