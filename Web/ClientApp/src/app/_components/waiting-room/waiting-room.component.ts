import { UserStorageService } from './../../_services/storage-services/user-storage.service';
import { GameStorageService } from './../../_services/storage-services/game-storage.service';
import { SidebarSettings } from 'src/app/_models/sidebarSettings';
import { UtilityService } from './../../_services/utility.service';
import { CardValue, GameType, PlayersSetup } from './../../_models/enums';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Game } from 'src/app/_models/game';
import { User } from 'src/app/_models/user';
import { HubService } from 'src/app/_services/hub.service';
import { Router } from '@angular/router';
import { Player } from 'src/app/_models/player';
import { takeWhile } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { ModalService } from 'src/app/_services/modal.service';

@Component({
  selector: 'app-waiting-room',
  templateUrl: './waiting-room.component.html',
  styleUrls: ['./waiting-room.component.css']
})
export class WaitingRoomComponent implements OnInit, OnDestroy {
  private _isAlive: boolean = true;

  activeGame: Game;
  sidebarSettings: SidebarSettings;
  currentUser: User;

  constructor(
    private _hubService: HubService,
    private _modalService: ModalService,
    private _utilityService: UtilityService,
    private _toastrService: ToastrService,
    private _router: Router,
    private _gameStorageService: GameStorageService,
    private _userStorageService: UserStorageService
  ) {}

  ngOnInit() {
    this.sidebarSettings = this._utilityService.sidebarSettings;
    this._gameStorageService.activeGame.pipe(takeWhile(() => this._isAlive)).subscribe(game => {
      this.activeGame = game;
    });

    this._userStorageService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });
  }

  getBannedCardName(bannedCard: CardValue) {
    return this._utilityService.getBannedCardName(bannedCard);
  }

  changeTeam(currentTeamNumber: number, isIncrement: boolean) {
    var teamNumber = isIncrement ? ++currentTeamNumber : --currentTeamNumber;
    if (teamNumber < 1 || teamNumber > 5) return;

    this._hubService.changeTeam(this.activeGame.id, teamNumber);
  }

  leaveWaitingRoom() {
    this._router.navigateByUrl('/');
  }

  joinGame() {
    if (this.activeGame.players.length >= this.activeGame.gameSetup.maxNumberOfPlayers) {
      this._toastrService.info(`Maximum number of players reached (${this.activeGame.gameSetup.maxNumberOfPlayers}).`);
      return;
    }
    this._hubService.joinGame(this.activeGame.id, '');
  }

  userIsSpectator() {
    const exists = this.activeGame.spectators.find(spectator => {
      return spectator.user.name === this.currentUser.name;
    });
    return exists != null;
  }

  getStyleJoinGameButton() {
    var obj: any = {};
    if (this.activeGame.players.length >= this.activeGame.gameSetup.maxNumberOfPlayers) {
      obj.opacity = '0.5';
    } else {
      obj.opacity = '1';
    }
    if (!this.userIsSpectator()) {
      obj.opacity = '0.5';
    }
    return obj;
  }

  openBanCardsDialog() {
    var banCardsModal = this._modalService.displayPickCardsToBanModal();
    banCardsModal.componentInstance.bannedCards = Object.assign([], this.activeGame.gameSetup.bannedCards);
    banCardsModal.result.then((bannedCards: CardValue[]) => {
      this.activeGame.gameSetup.bannedCards = bannedCards;
      this.updateGameSetup();
    });
  }

  getGameTypePlaceholder() {
    return this.activeGame.gameSetup.gameType == GameType.normal ? 'Normal' : 'Special Wild Cards';
  }
  getPlayerSetupPlaceholder() {
    return this.activeGame.gameSetup.playersSetup == PlayersSetup.individual ? 'Individual' : 'Teams';
  }

  openGameSetupDialog() {
    this._modalService.displayGameSetupModal();
  }
  startGame() {
    if (this.activeGame.players.length < 2) {
      this._toastrService.info('Minimum 2 players to start the game.');
      return;
    }

    if (this.activeGame.gameSetup.playersSetup == PlayersSetup.teams) {
      var allteams = this.activeGame.players.map(x => x.teamNumber);
      allteams = allteams.filter((value, index, self) => {
        return self.indexOf(value) === index;
      });
      if (allteams.length < 2) {
        this._toastrService.error("Can't start the team game with only one team.");
        return;
      }
    }

    this._hubService.startGame(this.activeGame.id);
  }
  kickPlayerFromGame(player: Player) {
    const cfrm = confirm('Really kick this player? ' + player.user.name);
    if (cfrm) {
      this._hubService.kickPlayerFromGame(this.activeGame.id, player.user);
    }
  }

  updateGameSetup() {
    this._hubService.updateGameSetup(this.activeGame.id, this.activeGame.gameSetup);
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
