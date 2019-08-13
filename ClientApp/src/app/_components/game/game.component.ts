import { BlackjackComponent } from './../_modals/Blackjack/Blackjack.component';
import { PickCharityCardsComponent } from './../_modals/pick-charity-cards/pick-charity-cards.component';
import { GameInfoComponent } from './../_modals/game-info/game-info.component';
import { CardValue, TypeOfMessage } from './../../_models/enums';
import { Component, OnInit } from '@angular/core';
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

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  private _timer: number;
  private _interval: number;
  private _gameEnded = false;

  isSidebarOpen = false;
  keepSidebarOpen: boolean;
  currentUser: User;
  game: Game;
  numberUnreadMessages = 0;
  myCards: Card[];
  mustCallUno = false;
  countdown = 2000;
  gameLog: string[];

  constructor(private _hubService: HubService, private _modalService: NgbModal, private _toastrService: ToastrService) {}

  ngOnInit() {
    this._hubService.activeGame.subscribe(game => {
      if (game === null) {
        return;
      }
      this.game = game;
      if (this.game.gameEnded && !this._gameEnded) {
        this._gameEnded = true;
        const message = `Game ended!`;
        alert(message);
      }
    });

    this._hubService.gameLog.subscribe(gameLog => {
      this.gameLog = gameLog;
    });

    this._hubService.mustCallUno.subscribe(() => {
      console.log('must call uno........' + new Date());
      return;
      // this.callUno(false);
      // this.mustCallUno = true;
      // this._interval = window.setInterval(() => {
      //   this.countdown -= 100;
      // }, 100);
      // this._timer = window.setTimeout(() => {
      //   if (this.mustCallUno) {
      //     if (!this.game.gameEnded) {
      //       this.drawCard(2, false);
      //       this.callUno(false);
      //       this._hubService.sendMessageToGameChat('<--- Forgot to call uno! Drawing 2 cards.', TypeOfMessage.chat);
      //     }
      //   }
      // }, 2000);
    });

    this._hubService.currentUser.subscribe(user => {
      this.currentUser = user;
    });

    this._hubService.myHand.subscribe((myCards: Card[]) => {
      if (this.game === null) {
        return;
      }
      this.myCards = myCards;
    });

    this._hubService.gameChatMessages.subscribe(messages => {
      if (!this.isSidebarOpen) {
        this.numberUnreadMessages++;
      }
    });
  }

  callUno(playerCalled: boolean) {
    this.mustCallUno = false;
    window.clearTimeout(this._timer);
    window.clearInterval(this._interval);
    this._timer = null;
    this._interval = null;
    this.countdown = 2000;
    if (playerCalled) {
      this._hubService.sendMessageToGameChat('UNO', TypeOfMessage.chat);
    }
  }

  playCard(card: Card) {
    if (this.mustCallUno) {
      return;
    }
    if (card.value === CardValue.stealTurn && card.color == this.game.lastCardPlayed.color) {
      this._hubService.playCard(card.id, card.color);
      return;
    }
    if (this.game.playerToPlay.user.name !== this.currentUser.name) {
      return;
    }

    if (card.value === CardValue.magneticPolarity && this.game.lastCardPlayed.wasWildCard === false) {
      this._toastrService.info('Magnetic polarity can be played only if last card played was a wildcard.', '', { timeOut: 3000 });
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

  toggleKeepSidebarOpen(event) {
    this.keepSidebarOpen = event;
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
}
