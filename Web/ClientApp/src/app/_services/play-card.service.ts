import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/_models/user';
import { ModalService } from 'src/app/_services/modal.service';
import { HubService } from './hub.service';
import { Injectable } from '@angular/core';
import { Card } from '../_models/card';
import { CardValue, CardColor, PlayersSetup } from '../_models/enums';
import { Game } from '../_models/game';

@Injectable({
  providedIn: 'root'
})
export class PlayCardService {
  constructor(private _hubService: HubService, private _modalService: ModalService, private _toastrService: ToastrService) {}

  playCard(cardPlayed: Card, game: Game, currentUser: User, myCards: Card[]) {
    if (
      cardPlayed.value === CardValue.stealTurn &&
      (cardPlayed.color == game.lastCardPlayed.color || game.lastCardPlayed.value == cardPlayed.value)
    ) {
      this._hubService.playCard(cardPlayed.id, cardPlayed.color);
      return;
    }

    if (
      game.gameSetup.matchingCardStealsTurn &&
      cardPlayed.color == game.lastCardPlayed.color &&
      game.lastCardPlayed.value == cardPlayed.value
    ) {
      this._hubService.playCard(cardPlayed.id, cardPlayed.color);
      return;
    }

    if (game.playerToPlay.user.name !== currentUser.name) {
      return;
    }

    if (
      cardPlayed.color != CardColor.wild &&
      (cardPlayed.color != game.lastCardPlayed.color && cardPlayed.value != game.lastCardPlayed.value)
    ) {
      return;
    }

    if (cardPlayed.color == CardColor.wild && game.gameSetup.wildCardCanBePlayedOnlyIfNoOtherOptions) {
      if (
        myCards.some(x => x.color == game.lastCardPlayed.color) ||
        (myCards.some(x => x.value == game.lastCardPlayed.value) && !game.lastCardPlayed.wasWildCard)
      ) {
        this._toastrService.info("You can play a wildcard only as the last option (can't play anything else).");
        return;
      }
    }

    if (
      (cardPlayed.value === CardValue.magneticPolarity || cardPlayed.value === CardValue.doubleDraw) &&
      game.lastCardPlayed.wasWildCard === false
    ) {
      this._toastrService.info('This card can only be played if the last card on the table is a wildcard.');
      return;
    }

    if (cardPlayed.value == CardValue.charity) {
      if (myCards.length < 3) {
        this._toastrService.info('This card can only be played if the number of cards on hand is 3 and more.');
        return;
      }
    }

    if (cardPlayed.value == CardValue.promiseKeeper) {
      var numberOfCardsWithoutWild = myCards.filter((card: Card) => card.color != CardColor.wild).length;
      if (numberOfCardsWithoutWild == 0) {
        this._toastrService.info('This card can only be played if the number of colored cards (excluding wild) on hand is 1 and more.');
        return;
      }
    }

    if (cardPlayed.color != CardColor.wild) {
      this._hubService.playCard(cardPlayed.id);
      return;
    }

    //wild cards
     var colorModal= this._modalService.displayPickColorModal();
     if (!cardPlayed.requirePickColor) {
      colorModal.close(game.lastCardPlayed.color);
     }
     colorModal.result.then(pickedColor => {
        if (cardPlayed.requireTargetPlayer) {
          const playerModal = this._modalService.displayPickPlayerModal();
          playerModal.componentInstance.players = game.players;
          playerModal.componentInstance.currentUser = currentUser;
          if(game.players.length==2){
            var opponent=game.players.find(x=>x.user.name!=currentUser.name);
            playerModal.close(opponent.id);
          }
          playerModal.result.then((playerId: string) => {
            if (cardPlayed.value == CardValue.duel) {
              this._modalService.displayPickDuelNumbers().result.then((duelNumbers: number[]) => {
                this._hubService.playCard(cardPlayed.id, pickedColor, playerId, null, duelNumbers);
                return;
              });
            } else if (cardPlayed.value == CardValue.assassinate) {
              this._modalService.displayPickAnyCardModal().result.then((selectedCard:CardValue) => {
                this._hubService.playCard(cardPlayed.id, pickedColor, playerId, null, null,null,0,null,null,null,false,selectedCard);
                return;
              });
            } else if (cardPlayed.value == CardValue.charity) {
              const modalRef = this._modalService.displayPickCharityCardsModal();
              modalRef.componentInstance.cards = myCards.filter((card: Card) => card.id != cardPlayed.id);
              modalRef.result.then((charityCardsIds: string[]) => {
                this._hubService.playCard(cardPlayed.id, pickedColor, playerId, null, null, charityCardsIds);
                return;
              });
            }else if (cardPlayed.value == CardValue.gambling) {
              if (game.gameSetup.playersSetup == PlayersSetup.teams) {
                var targetedPlayerTeam = game.players.find(x => x.id == playerId).teamNumber;
                var myTeam = game.players.find(x => x.user.name == currentUser.name).teamNumber;
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
        }else if (cardPlayed.value === CardValue.devilsDeal) {
          const specialEffectModal = this._modalService.displayActivateSpecialEffect();
          specialEffectModal.result.then((activateEffect: boolean) => {
            this._hubService.playCard(cardPlayed.id, pickedColor, null, null, null,null,0,null,null,null,activateEffect);
            return;
          });
        }else if (cardPlayed.value === CardValue.deathSentence) {
          this._modalService.displayActivateSpecialEffect().result.then((activateEffect: boolean) => {
            if(activateEffect){
              var modalRef= this._modalService.displayPickWildCardModal();
              modalRef.componentInstance.bannedCards = game.gameSetup.bannedCards;
              modalRef.result.then((selectedCard:CardValue) => {
                this._hubService.playCard(cardPlayed.id, pickedColor, null, null, null,null,0,null,null,null,activateEffect, selectedCard);
                return;
              });
            }else{
              this._hubService.playCard(cardPlayed.id, pickedColor, null, null, null,null,0,null,null,null,activateEffect);
              return;
            }
          });
        } else if (cardPlayed.value === CardValue.graveDigger) {
          const digModal = this._modalService.displayDigCardModal();
          digModal.componentInstance.discardedPile = game.discardedPile;
          digModal.result.then((cardToDigId: string) => {
            this._hubService.playCard(cardPlayed.id, pickedColor, null, cardToDigId);
            return;
          });
        } else if (cardPlayed.value == CardValue.summonWildcard) {
          var modalRef= this._modalService.displayPickWildCardModal();
          modalRef.componentInstance.bannedCards = game.gameSetup.bannedCards;
          modalRef.result.then((selectedCard:CardValue) => {
            this._hubService.playCard(cardPlayed.id, pickedColor, null, null, null,null,0,null,null,null,false,selectedCard);
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
          modalRef.componentInstance.cards = myCards.filter((card: Card) => card.color != CardColor.wild);
          modalRef.result.then((promisedCardId: string) => {
            this._hubService.playCard(cardPlayed.id, pickedColor, null, null, null, null, 0, null, promisedCardId);
            return;
          });
        } else {
          this._hubService.playCard(cardPlayed.id, pickedColor);
          return;
        }
      });
  }
}
