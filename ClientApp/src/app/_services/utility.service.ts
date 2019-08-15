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
    if (this._sidebarSettings == null) {
      let keepSidebarOpen=true;
      let sidebarSize=30;
      if (window.innerWidth < 768) {
        keepSidebarOpen = false;
        sidebarSize = 50;
      }
      this._sidebarSettings = { sidebarSize: sidebarSize, muteServer: false, muteSpectators: false, keepSidebarOpen: keepSidebarOpen };
    }
    
    return this._sidebarSettings;
  }

  getBannedCardName(bannedCard: CardValue) {
    return this.allCards.find(c => c.value == bannedCard).key;
  }
}
