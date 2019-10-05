import { KeyValue } from '@angular/common';
import { UserStorageService } from './../../_services/storage-services/user-storage.service';
import { GameStorageService } from './../../_services/storage-services/game-storage.service';
import { ModalService } from '../../_services/modal.service';
import { ChatMessage } from './../../_models/chatMessage';
import { UtilityService } from './../../_services/utility.service';
import { CardValue, TypeOfMessage, PlayersSetup } from './../../_models/enums';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { User } from 'src/app/_models/user';
import { Game } from 'src/app/_models/game';
import { Card } from 'src/app/_models/card';
import { HubService } from 'src/app/_services/hub.service';
import { CardColor, Direction } from 'src/app/_models/enums';
import { ToastrService } from 'ngx-toastr';
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
    private _toastrService: ToastrService,
    private _utilityService: UtilityService,
    private _router: Router,
    private _gameStorageService: GameStorageService,
    private _userStorageService: UserStorageService
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
    if (
      cardPlayed.value === CardValue.stealTurn &&
      (cardPlayed.color == this.game.lastCardPlayed.color || this.game.lastCardPlayed.value == cardPlayed.value)
    ) {
      this._hubService.playCard(cardPlayed.id, cardPlayed.color);
      return;
    }

    if (
      this.game.gameSetup.matchingCardStealsTurn &&
      cardPlayed.color == this.game.lastCardPlayed.color &&
      this.game.lastCardPlayed.value == cardPlayed.value
    ) {
      this._hubService.playCard(cardPlayed.id, cardPlayed.color);
      return;
    }

    if (this.game.playerToPlay.user.name !== this.currentUser.name) {
      return;
    }

    if (
      cardPlayed.color != CardColor.wild &&
      (cardPlayed.color != this.game.lastCardPlayed.color && cardPlayed.value != this.game.lastCardPlayed.value)
    ) {
      return;
    }

    if (cardPlayed.color == CardColor.wild && this.game.gameSetup.wildCardCanBePlayedOnlyIfNoOtherOptions) {
      if (
        this.myCards.some(x => x.color == this.game.lastCardPlayed.color) ||
        this.myCards.some(x => x.value == this.game.lastCardPlayed.value)
      ) {
        this._toastrService.info("You can play a wildcard only as the last option (can't play anything else).");
        return;
      }
    }

    if (
      (cardPlayed.value === CardValue.magneticPolarity || cardPlayed.value === CardValue.doubleDraw) &&
      this.game.lastCardPlayed.wasWildCard === false
    ) {
      this._toastrService.info('This card can only be played if the last card on the table is a wildcard.');
      return;
    }

    if (cardPlayed.value == CardValue.charity) {
      if (this.myCards.length < 3) {
        this._toastrService.info('This card can only be played if the number of cards on hand is 3 and more.');
        return;
      }
    }

    if (cardPlayed.value == CardValue.promiseKeeper) {
      var numberOfCardsWithoutWild = this.myCards.filter((card: Card) => card.color != CardColor.wild).length;
      if (numberOfCardsWithoutWild == 0) {
        this._toastrService.info('This card can only be played if the number of colored cards (excluding wild) on hand is 1 and more.');
        return;
      }
    }

    if (cardPlayed.requirePickColor) {
      this._modalService.displayPickColorModal().result.then(pickedColor => {
        if (cardPlayed.requireTargetPlayer) {
          const playerModal = this._modalService.displayPickPlayerModal();
          playerModal.componentInstance.players = this.game.players;
          playerModal.componentInstance.currentUser = this.currentUser;
          playerModal.result.then((playerId: string) => {
            if (cardPlayed.value == CardValue.duel) {
              this._modalService.displayPickDuelNumbers().result.then((duelNumbers: number[]) => {
                this._hubService.playCard(cardPlayed.id, pickedColor, playerId, null, duelNumbers);
                return;
              });
            } else if (cardPlayed.value == CardValue.charity) {
              const modalRef = this._modalService.displayPickCharityCardsModal();
              modalRef.componentInstance.cards = this.myCards.filter((card: Card) => card.id != cardPlayed.id);
              modalRef.result.then((charityCardsIds: string[]) => {
                this._hubService.playCard(cardPlayed.id, pickedColor, playerId, null, null, charityCardsIds);
                return;
              });
            } else if (cardPlayed.value == CardValue.gambling) {
              if (this.game.gameSetup.playersSetup == PlayersSetup.teams) {
                var targetedPlayerTeam = this.game.players.find(x => x.id == playerId).teamNumber;
                var myTeam = this.game.players.find(x => x.user.name == this.currentUser.name).teamNumber;
                if (targetedPlayerTeam == myTeam) {
                  this._toastrService.error("You can't pick your teammate for the gambling card.");
                  return;
                }
              }
              const modalRef = this._modalService.displayGuessOdEvenNumbersModal();
              modalRef.result.then((guessOddOrEven: string) => {
                this._hubService.playCard(cardPlayed.id, pickedColor, playerId, null, null, null, 0, null, null, guessOddOrEven);
                return;
              });
            } else {
              this._hubService.playCard(cardPlayed.id, pickedColor, playerId);
              return;
            }
          });
        } else if (cardPlayed.value === CardValue.graveDigger) {
          const digModal = this._modalService.displayDigCardModal();
          digModal.componentInstance.discardedPile = this.game.discardedPile;
          digModal.result.then((cardToDigId: string) => {
            this._hubService.playCard(cardPlayed.id, pickedColor, null, cardToDigId);
            return;
          });
        } else if (cardPlayed.value === CardValue.blackjack) {
          this._modalService.displayBlackjackModal().result.then(blackjackNumber => {
            this._hubService.playCard(cardPlayed.id, pickedColor, null, null, null, null, blackjackNumber);
            return;
          });
        } else if (cardPlayed.value === CardValue.discardNumber) {
          this._modalService.displayPickNumbersToDiscardModal().result.then((numbersToDiscard: number[]) => {
            this._hubService.playCard(cardPlayed.id, pickedColor, null, null, null, null, 0, numbersToDiscard);
            return;
          });
        } else if (cardPlayed.value === CardValue.promiseKeeper) {
          var modalRef = this._modalService.displayPickPromiseKeeperCardModal();
          modalRef.componentInstance.cards = this.myCards.filter((card: Card) => card.color != CardColor.wild);
          modalRef.result.then((promisedCardId: string) => {
            this._hubService.playCard(cardPlayed.id, pickedColor, null, null, null, null, 0, null, promisedCardId);
            return;
          });
        } else {
          this._hubService.playCard(cardPlayed.id, pickedColor);
          return;
        }
      });
    } else {
      this._hubService.playCard(cardPlayed.id);
      return;
    }
  }

  exitGame() {
    if (!this.game.isTournamentGame) {
      this._router.navigateByUrl('/');
    } else {
      this._router.navigateByUrl('/tournament');
    }
  }

  getSidebarClass() {
    var classes = [];
    classes.push(`fill-viewport-${this.sidebarSettings.sidebarSize}`);
    return classes;
  }

  getSidebarBackgroundColor() {
    return this._utilityService.getSidebarBackgroundColor();
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
    return this.game.direction === Direction.right ? '-->' : '<--';
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
