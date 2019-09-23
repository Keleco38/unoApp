import { UserSettings } from './../../_models/userSettings';
import { UtilityService } from './../../_services/utility.service';
import { ContactFormComponent } from './../_modals/contact-form/contact-form.component';
import { Component, OnInit } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserSettingsComponent } from '../_modals/user-settings/user-settings.component';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  animations: [
    trigger('smoothCollapse', [
      state(
        'initial',
        style({
          height: '0',
          overflow: 'hidden',
          opacity: '0'
        })
      ),
      state(
        'final',
        style({
          overflow: 'hidden',
          opacity: '1'
        })
      ),
      transition('initial=>final', animate('300ms')),
      transition('final=>initial', animate('300ms'))
    ])
  ]
})
export class NavbarComponent implements OnInit {
  userSettings: UserSettings;
  navbarOpen = false;
  constructor(private _modalService: NgbModal, private _utilityService: UtilityService) {}

  ngOnInit() {
    this.userSettings = this._utilityService.userSettings;
  }

  toggleNavbar() {
    if (window.innerWidth < 992) {
      this.navbarOpen = !this.navbarOpen;
    }
  }

  openContactModal() {
    this._modalService.open(ContactFormComponent, { backdrop: 'static' });
  }

  openSettings() {
    this._modalService.open(UserSettingsComponent);
  }
}
