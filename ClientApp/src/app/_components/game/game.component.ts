import { MyHand } from './../../_models/myHand';
import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/_models/user';
import { Game } from 'src/app/_models/game';
import { Card } from 'src/app/_models/card';
import { HubService } from 'src/app/_services/hub.service';
import { Router } from '@angular/router';
import { CardColor, Direction } from 'src/app/_models/enums';
import { NgbPopover, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PickColorComponent } from '../_modals/pick-color/pick-color.component';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  @ViewChild('cardsPlayedPopover')
  private cardsPlayedPopover: NgbPopover;

  isGameChatSidebarOpen = false;
  currentUser: User;
  game: Game;
  numberUnreadMessages = 0;
  myHand: MyHand;

  constructor(private _hubService: HubService, private _modalService: NgbModal) {}

  ngOnInit() {
    this._hubService.activeGame.subscribe(game => {
      if (this.game != null && !this.game.gameEnded && game != null && game.gameEnded) {
        const message = `Game ended! Winner ${game.players.find(x => x.numberOfCards === 0).user.name}`;
        alert(message);
      }
      this.game = game;
    });

    this._hubService.currentUser.subscribe(user => {
      this.currentUser = user;
    });

    this._hubService.myHand.subscribe(myHand => {
      this.myHand = myHand;
    });

    this._hubService.gameChatMessages.subscribe(messages => {
      if (!this.isGameChatSidebarOpen) {
        this.numberUnreadMessages++;
      }
    });
  }

  playCard(card: Card) {
    if (card.color === CardColor.wild) {
      this._modalService.open(PickColorComponent).result.then(
        result => {
          this._hubService.playCard(card, result);
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

  drawCard() {
    this._hubService.drawCard();
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
