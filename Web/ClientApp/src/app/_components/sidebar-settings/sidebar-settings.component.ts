import { Component, OnInit } from '@angular/core';
import { UtilityService } from 'src/app/_services/utility.service';
import { SidebarSettings } from 'src/app/_models/sidebarSettings';

@Component({
  selector: 'app-sidebar-settings',
  templateUrl: './sidebar-settings.component.html',
  styleUrls: ['./sidebar-settings.component.css']
})
export class SidebarSettingsComponent implements OnInit {
  sidebarSettings: SidebarSettings;

  constructor(private _utilityService: UtilityService) {}

  ngOnInit(): void {
    this.sidebarSettings = this._utilityService.sidebarSettings;
  }

  saveSidebarSettings() {
    this._utilityService.saveSidebarSettings();
  }
}
