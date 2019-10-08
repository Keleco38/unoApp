import { PlayCardService } from './../../_services/play-card.service';
import { KeyValue } from '@angular/common';
import { UserStorageService } from './../../_services/storage-services/user-storage.service';
import { GameStorageService } from './../../_services/storage-services/game-storage.service';
import { ModalService } from '../../_services/modal.service';
import { ChatMessage } from './../../_models/chatMessage';
import { UtilityService } from './../../_services/utility.service';
import { TypeOfMessage } from './../../_models/enums';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { User } from 'src/app/_models/user';
import { Game } from 'src/app/_models/game';
import { Card } from 'src/app/_models/card';
import { HubService } from 'src/app/_services/hub.service';
import { CardColor, Direction } from 'src/app/_models/enums';
import { SidebarSettings } from 'src/app/_models/sidebarSettings';
import { takeWhile } from 'rxjs/operators';
import { Router } from '@angular/router';
import { UserSettings } from 'src/app/_models/userSettings';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit, OnDestroy {
  private _timer: number;
  private _mustCallUno: boolean = false;
  private _isAlive: boolean = true;

  isSidebarOpen = false;
  closeOnClickOutside = true;
  currentUser: User;
  game: Game;
  numberUnreadMessages = 0;
  myCards: Card[];
  sidebarSettings: SidebarSettings;
  gameLog: string[];
  userSettings: UserSettings;
  spectatorsViewCardsAndUser: KeyValue<string, Card[]>[];

  constructor(
    private _hubService: HubService,
    private _modalService: ModalService,
    private _utilityService: UtilityService,
    private _router: Router,
    private _gameStorageService: GameStorageService,
    private _userStorageService: UserStorageService,
    private _playCardService: PlayCardService
  ) {}

  ngOnInit() {
    this.userSettings = this._utilityService.userSettings;
    this.sidebarSettings = this._utilityService.sidebarSettings;
    this._gameStorageService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      if (game === null) {
        return;
      }
      this.game = game;
    });

    this._hubService.updateGameEnded.pipe(takeWhile(() => this._isAlive)).subscribe(gameEndedResult => {
      this.isSidebarOpen = false;
    });

    this._gameStorageService.gameLog.pipe(takeWhile(() => this._isAlive)).subscribe(gameLog => {
      this.gameLog = gameLog;
    });

    this._gameStorageService.mustCallUno.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this._mustCallUno = true;
      window.clearTimeout(this._timer);
      this._timer = window.setTimeout(() => {
        this.callUno(false);
      }, 2000);
    });

    this._userStorageService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });

    this._gameStorageService.myHand.pipe(takeWhile(() => this._isAlive)).subscribe((myCards: Card[]) => {
      if (this.game === null) {
        return;
      }
      this.myCards = myCards;
    });

    this._gameStorageService.spectatorsViewHandsAndUser
      .pipe(takeWhile(() => this._isAlive))
      .subscribe((spectatorsViewCardsAndUser: KeyValue<string, Card[]>[]) => {
        if (this.game === null) {
          return;
        }
        this.spectatorsViewCardsAndUser = spectatorsViewCardsAndUser;
      });

    this._gameStorageService.gameChatNumberOfMessage.pipe(takeWhile(() => this._isAlive)).subscribe((message: ChatMessage) => {
      if (!this.isSidebarOpen) {
        if (message.typeOfMessage == TypeOfMessage.server && this.sidebarSettings.muteServer) return;
        if (message.typeOfMessage == TypeOfMessage.spectators && this.sidebarSettings.muteSpectators) return;
        this.numberUnreadMessages++;
      }
    });
  }

  isSpectator() {
    const exists = this.game.spectators.find(spectator => {
      return spectator.user.name === this.currentUser.name;
    });
    return exists != null;
  }

  callUno(unoCalled: boolean) {
    this._mustCallUno = false;
    window.clearTimeout(this._timer);
    this._hubService.checkUnoCall(unoCalled);
  }

  seeTeammatesCards() {
    this._hubService.seeTeammatesCards();
  }

  playCard(cardPlayed: Card) {
    if (this._mustCallUno) {
      return;
    }
    this._playCardService.playCard(cardPlayed, this.game, this.currentUser, this.myCards);
  }

  dropdownOpenChange(event) {
    this.closeOnClickOutside = !event;
  }

  exitGame() {
    if (!this.game.isTournamentGame) {
      this._router.navigateByUrl('/');
    } else {
      this._router.navigateByUrl('/tournament');
    }
  }

  getSidebarBackgroundClass() {
    if (this.userSettings.useDarkTheme) {
      return 'bg-secondary';
    }
    return 'bg-white';
  }

  drawCard() {
    if (this.game.playerToPlay.user.name != this.currentUser.name) {
      return;
    }
    this._hubService.drawCard();
  }

  openGameInfoModal() {
    this._modalService.displayGameInfoModal();
  }

  toggleGameChatSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
    this.numberUnreadMessages = 0;
  }

  getBorderColor() {
    switch (this.game.lastCardPlayed.color) {
      case CardColor.blue:
        return '#00C3E5';
      case CardColor.green:
        return '#2FE29B';
      case CardColor.red:
        return '#F56462';
      case CardColor.yellow:
        return '#F7E359';
    }
  }

  getDirectionStringFromGame() {
    return this.game.direction === Direction.right ? '->' : '<-';
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
