import { CardValue } from './../../_models/enums';
import { Hand } from '../../_models/hand';
import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/_models/user';
import { Game } from 'src/app/_models/game';
import { Card } from 'src/app/_models/card';
import { HubService } from 'src/app/_services/hub.service';
import { Router } from '@angular/router';
import { CardColor, Direction } from 'src/app/_models/enums';
import { NgbPopover, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PickColorComponent } from '../_modals/pick-color/pick-color.component';
import { PickPlayerComponent } from '../_modals/pick-player/pick-player.component';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  private _timer: NodeJS.Timer;
  private _interval: NodeJS.Timer;
  private _hasCalledUno: boolean;
  private _hasPlayed: boolean;
  private _gameEnded = false;

  isGameChatSidebarOpen = false;
  currentUser: User;
  game: Game;
  numberUnreadMessages = 0;
  myHand: Hand;
  mustCallUno = false;
  countdown = 2000;

  constructor(private _hubService: HubService, private _modalService: NgbModal) {}

  ngOnInit() {
    this._hubService.activeGame.subscribe(game => {
      if (game === null) {
        return;
      }
      this.game = game;
      if (this.game.gameEnded && !this._gameEnded) {
        this._gameEnded = true;
        const message = `Game ended! Winner ${game.players.find(x => x.numberOfCards === 0).user.name}`;
        alert(message);
      }
    });

    this._hubService.currentUser.subscribe(user => {
      this.currentUser = user;
    });

    this._hubService.myHand.subscribe(myHand => {
      if (this.game === null) {
        return;
      }
      this.myHand = myHand;
      if (this.game.lastCardPlayed.playerPlayed === this.currentUser.name && this.myHand.cards.length === 1 && this._hasPlayed && !this.mustCallUno) {
        this._hasCalledUno = false;
        this.mustCallUno = true;
        this._interval = setInterval(() => {
          this.countdown -= 100;
        }, 100);

        this._timer = setTimeout(() => {
          if (!this._hasCalledUno) {
            this.drawCard(2, false);
            this.callUno(false);
            this._hubService.sendMessageToGameChat('<--- Forgot to call uno! Drawing 2 cards.');
          }
        }, 2000);
      }
    });

    this._hubService.gameChatMessages.subscribe(messages => {
      if (!this.isGameChatSidebarOpen) {
        this.numberUnreadMessages++;
      }
    });
  }

  callUno(playerCalled: boolean) {
    this._hasCalledUno = true;
    this.mustCallUno = false;
    this.countdown = 2000;
    clearTimeout(this._timer);
    clearInterval(this._interval);
    this._hasPlayed = false;
    if (playerCalled) {
      this._hubService.sendMessageToGameChat('UNO');
    }
  }

  playCard(card: Card) {
    if (this.mustCallUno) {
      return;
    }
    if (this.game.playerToPlay.user.name !== this.currentUser.name && card.value !== CardValue.stealTurn) {
      return;
    }
    if (card.value !== CardValue.graveDigger) {
      this._hasPlayed = true;
    } else {
      this._hasPlayed = false;
    }
    if (card.color === CardColor.wild) {
      this._modalService.open(PickColorComponent).result.then(
        pickedColor => {
          if (card.value === CardValue.swapHands || card.value === CardValue.doubleEdge || card.value === CardValue.judgement || card.value === CardValue.inspectHand) {
            const playerModal = this._modalService.open(PickPlayerComponent);
            playerModal.componentInstance.players = this.game.players;
            playerModal.componentInstance.currentUser = this.currentUser;
            playerModal.result.then(
              playerName => {
                this._hubService.playCard(card, pickedColor, playerName);
              },
              dismissed => {}
            );
          } else {
            this._hubService.playCard(card, pickedColor);
          }
        },
        dismissed => {}
      );
    } else {
      this._hubService.playCard(card, card.color);
    }
  }

  exitGame() {
    this._hubService.exitGame();
  }

  drawCard(count: number, changeTurn: boolean) {
    this._hubService.drawCard(count, changeTurn);
  }

  toggleGameChatSidebar() {
    this.isGameChatSidebarOpen = !this.isGameChatSidebarOpen;
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
    return this.game.direction === Direction.right ? '------------>>>' : '<<<------------';
  }
}
