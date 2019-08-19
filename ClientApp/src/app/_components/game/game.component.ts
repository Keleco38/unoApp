import { ChatMessage } from './../../_models/chatMessage';
import { UtilityService } from './../../_services/utility.service';
import { BlackjackComponent } from './../_modals/blackjack/blackjack.component';
import { PickCharityCardsComponent } from './../_modals/pick-charity-cards/pick-charity-cards.component';
import { GameInfoComponent } from './../_modals/game-info/game-info.component';
import { CardValue, TypeOfMessage } from './../../_models/enums';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { User } from 'src/app/_models/user';
import { Game } from 'src/app/_models/game';
import { Card } from 'src/app/_models/card';
import { HubService } from 'src/app/_services/hub.service';
import { CardColor, Direction } from 'src/app/_models/enums';
import { NgbPopover, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PickColorComponent } from '../_modals/pick-color/pick-color.component';
import { PickPlayerComponent } from '../_modals/pick-player/pick-player.component';
import { DigCardComponent } from '../_modals/dig-card/dig-card.component';
import { PickDuelNumbersComponent } from '../_modals/pick-duel-numbers/pick-duel-numbers.component';
import { PickNumbersToDiscardComponent } from '../_modals/pick-numbers-to-discard/pick-numbers-to-discard.component';
import { ToastrService } from 'ngx-toastr';
import { SidebarSettings } from 'src/app/_models/sidebarSettings';
import { takeWhile } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit, OnDestroy {
  private _timer: number;
  private _gameEnded: boolean = false;
  private _mustCallUno: boolean = false;
  private _isAlive: boolean = true;

  isSidebarOpen = false;
  currentUser: User;
  game: Game;
  numberUnreadMessages = 0;
  myCards: Card[];
  sidebarSettings: SidebarSettings;
  gameLog: string[];

  constructor(
    private _hubService: HubService,
    private _modalService: NgbModal,
    private _toastrService: ToastrService,
    private _utilityService: UtilityService,
    private _router:Router
  ) {}

  ngOnInit() {
    this.sidebarSettings = this._utilityService.sidebarSettings;
    this._hubService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      if (game === null) {
        return;
      }
      this.game = game;
      if (this.game.gameEnded && !this._gameEnded) {
        this._gameEnded = true;
        alert('Game ended');
      }
    });

    this._hubService.gameLog.pipe(takeWhile(() => this._isAlive)).subscribe(gameLog => {
      this.gameLog = gameLog;
    });

    this._hubService.mustCallUno.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this._mustCallUno = true;
      window.clearTimeout(this._timer);
      this._timer = window.setTimeout(() => {
        if (this._mustCallUno) {
          if (!this.game.gameEnded) {
            this.drawCard(2, false);
            this.callUno(false);
            this._hubService.sendMessageToGameChat('<--- This person forgot to call uno! Drawing 2 cards.');
          }
        }
      }, 2000);
    });

    this._hubService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });

    this._hubService.myHand.pipe(takeWhile(() => this._isAlive)).subscribe((myCards: Card[]) => {
      if (this.game === null) {
        return;
      }
      this.myCards = myCards;
    });

    this._hubService.gameChatNumberOfMessages.pipe(takeWhile(() => this._isAlive)).subscribe((message: ChatMessage) => {
      if (!this.isSidebarOpen) {
        if (message.typeOfMessage == TypeOfMessage.server && this.sidebarSettings.muteServer) return;
        if (message.typeOfMessage == TypeOfMessage.spectators && this.sidebarSettings.muteSpectators) return;
        this.numberUnreadMessages++;
      }
    });
  }

  callUno(playerCalled: boolean) {
    if (!this._mustCallUno){
      return;
    }
    this._mustCallUno = false;
    window.clearTimeout(this._timer);
    if (playerCalled) {
      this._hubService.sendMessageToGameChat('UNO');
    }
  }

  playCard(card: Card) {
    if (this._mustCallUno) {
      return;
    }
    if (
      card.value === CardValue.stealTurn &&
      (card.color == this.game.lastCardPlayed.color || this.game.lastCardPlayed.value == card.value)
    ) {
      this._hubService.playCard(card.id, card.color);
      return;
    }
    if (this.game.playerToPlay.user.name !== this.currentUser.name) {
      return;
    }

    if (
      (card.value === CardValue.magneticPolarity || card.value === CardValue.doubleDraw) &&
      this.game.lastCardPlayed.wasWildCard === false
    ) {
      this._toastrService.info('This card only can be played if last card played is a wildcard.', '', { timeOut: 3000 });
      return;
    }

    if (card.color === CardColor.wild) {
      this._modalService.open(PickColorComponent).result.then(pickedColor => {
        if (
          card.value === CardValue.swapHands ||
          card.value === CardValue.doubleEdge ||
          card.value === CardValue.judgement ||
          card.value === CardValue.duel ||
          card.value === CardValue.inspectHand ||
          card.value === CardValue.charity ||
          card.value === CardValue.tricksOfTheTrade ||
          card.value === CardValue.fairPlay
        ) {
          const playerModal = this._modalService.open(PickPlayerComponent);
          playerModal.componentInstance.players = this.game.players;
          playerModal.componentInstance.currentUser = this.currentUser;
          playerModal.result.then((playerId: string) => {
            if (card.value == CardValue.duel) {
              this._modalService.open(PickDuelNumbersComponent).result.then((duelNumbers: number[]) => {
                this._hubService.playCard(card.id, pickedColor, playerId, null, duelNumbers);
                return;
              });
            } else if (card.value == CardValue.charity) {
              const modalRef = this._modalService.open(PickCharityCardsComponent);
              modalRef.componentInstance.cards = this.myCards;
              modalRef.result.then((charityCardsIds: string[]) => {
                this._hubService.playCard(card.id, pickedColor, playerId, null, null, charityCardsIds);
                return;
              });
            } else {
              this._hubService.playCard(card.id, pickedColor, playerId);
              return;
            }
          });
        } else if (card.value === CardValue.graveDigger) {
          const digModal = this._modalService.open(DigCardComponent);
          digModal.componentInstance.discardedPile = this.game.discardedPile;
          digModal.result.then((cardToDigId: string) => {
            this._hubService.playCard(card.id, pickedColor, null, cardToDigId);
            return;
          });
        } else if (card.value === CardValue.blackjack) {
          this._modalService.open(BlackjackComponent, { backdrop: 'static' }).result.then(blackjackNumber => {
            this._hubService.playCard(card.id, pickedColor, null, null, null, null, blackjackNumber);
            return;
          });
        } else if (card.value === CardValue.discardNumber) {
          this._modalService.open(PickNumbersToDiscardComponent).result.then(numbersToDiscard => {
            this._hubService.playCard(card.id, pickedColor, null, null, null, null, 0, numbersToDiscard);
            return;
          });
        } else {
          this._hubService.playCard(card.id, pickedColor);
          return;
        }
      });
    } else if (card.color == this.game.lastCardPlayed.color || card.value == this.game.lastCardPlayed.value) {
      this._hubService.playCard(card.id);
      return;
    }
  }

  exitGame() {
    this._hubService.exitGame();
    this._router.navigateByUrl('/');

  }

  getSidebarClass() {
    return `fill-viewport-${this.sidebarSettings.sidebarSize}`;
  }

  drawCard(count: number, changeTurn: boolean) {
    if (changeTurn && this.game.playerToPlay.user.name != this.currentUser.name) {
      return;
    }
    this._hubService.drawCard(count, changeTurn);
  }

  openGameInfoModal() {
    this._modalService.open(GameInfoComponent);
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
    return this.game.direction === Direction.right ? '-->' : '<--';
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
