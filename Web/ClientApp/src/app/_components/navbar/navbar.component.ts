import { ModalService } from 'src/app/_services/modal.service';
import { UserSettings } from './../../_models/userSettings';
import { UtilityService } from './../../_services/utility.service';
import { Component, OnInit } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { UserSettingsComponent } from '../_modals/user-settings/user-settings.component';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  userSettings: UserSettings;
  navbarOpen = false;
  constructor(private _modalService: ModalService, private _utilityService: UtilityService) {}

  ngOnInit() {
    this.userSettings = this._utilityService.userSettings;
  }

  toggleNavbar() {
    if (window.innerWidth < 992) {
      this.navbarOpen = !this.navbarOpen;
    }
  }


  openSettings() {
    this._modalService.displayUserSettingsModal();
  }
}
