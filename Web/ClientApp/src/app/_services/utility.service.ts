import { UserSettings } from './../_models/userSettings';
import { Injectable, OnInit } from '@angular/core';
import { KeyValue } from '@angular/common';
import { CardValue } from '../_models/enums';
import { SidebarSettings } from '../_models/sidebarSettings';

@Injectable({
  providedIn: 'root'
})
export class UtilityService {
  private _allCards: KeyValue<string, number>[] = [];
  private _sidebarSettings: SidebarSettings = null;
  private _userSettings: UserSettings = null;
  constructor() {}

  get allCards() {
    if (this._allCards.length === 0) {
      for (var enumMember in CardValue) {
        var isValueProperty = parseInt(enumMember, 10) >= 0;
        if (isValueProperty) {
          this._allCards.push({ key: CardValue[enumMember], value: parseInt(enumMember) });
        }
      }
    }
    return this._allCards;
  }

  get sidebarSettings(): SidebarSettings {
    if (this._sidebarSettings === null) {
      this._sidebarSettings = JSON.parse(localStorage.getItem('sidebar-settings'));
      if (this._sidebarSettings === null) {
        this._sidebarSettings = { sidebarSize: 50, muteServer: false, muteSpectators: false, keepSidebarOpen: false, showNavbar: false };
        this.saveSidebarSettings();
      }
    }
    return this._sidebarSettings;
  }

  get userSettings(): UserSettings {
    if (this._userSettings === null) {
      this._userSettings = JSON.parse(localStorage.getItem('user-settings'));
      if (this._userSettings === null) {
        this._userSettings = {
          notifyUserWhenHisTurnToPlay: false,
          blockedBuzzCommands: [],
          notifyWhenMentionedBuzz: false,
          notifyWhenMentionedToast: true
        };
        this.saveUserSettings();
      } else {
        if (this._userSettings.notifyWhenMentionedToast == null) {
          this._userSettings.notifyWhenMentionedToast = true;
          this.saveUserSettings();
        }
      }
    }
    return this._userSettings;
  }

  get isFirstTimeLunched() {
    let isFirstTimeLunched = JSON.parse(localStorage.getItem('is-first-time-lunched'));
    if (isFirstTimeLunched === null) return true;
    return isFirstTimeLunched;
  }

  getBannedCardName(bannedCard: CardValue) {
    return this.allCards.find(c => c.value == bannedCard).key;
  }

  updateFirstTimeLunched() {
    localStorage.setItem('is-first-time-lunched', JSON.stringify(false));
  }

  saveSidebarSettings() {
    localStorage.setItem('sidebar-settings', JSON.stringify(this._sidebarSettings));
  }
  saveUserSettings() {
    localStorage.setItem('user-settings', JSON.stringify(this._userSettings));
  }
}
