import { TournamentSetupComponent } from './../_modals/tournament-setup/tournament-setup.component';
import { Contestant } from './../../_models/contestant';
import { SidebarSettings } from 'src/app/_models/sidebarSettings';
import { UtilityService } from 'src/app/_services/utility.service';
import { Tournament } from './../../_models/tournament';
import { HubService } from 'src/app/_services/hub.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { takeWhile } from 'rxjs/operators';
import { User } from '../../_models/user';
import { Router } from '@angular/router';
import { GameType, CardValue } from '../../_models/enums';
import { ToastrService } from 'ngx-toastr';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PickBannedCardsComponent } from '../_modals/pick-banned-cards/pick-banned-cards.component';

@Component({
  selector: 'app-tournament-waiting-room',
  templateUrl: './tournament-waiting-room.component.html',
  styleUrls: ['./tournament-waiting-room.component.css']
})
export class TournamentWaitingRoomComponent implements OnInit, OnDestroy {
  private _isAlive: boolean = true;
  activeTournament: Tournament;
  sidebarSettings: SidebarSettings;
  currentUser: User;

  constructor(
    private _hubService: HubService,
    private _utilityService: UtilityService,
    private _router: Router,
    private _toastrService: ToastrService,
    private _modalService: NgbModal
  ) {}

  ngOnInit() {
    this.sidebarSettings = this._utilityService.sidebarSettings;

    this._hubService.activeTournament.pipe(takeWhile(() => this._isAlive)).subscribe(tournament => {
      this.activeTournament = tournament;
    });

    this._hubService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });
  }

  getStyleJoinGameButton() {
    var obj: any = {};
    if (this.activeTournament.contestants.length >= this.activeTournament.tournamentSetup.numberOfPlayers) {
      obj.opacity = '0.5';
    } else {
      obj.opacity = '1';
    }
    if (!this.userIsSpectator()) {
      obj.opacity = '0.5';
    }
    return obj;
  }

  getBannedCardName(bannedCard: CardValue) {
    return this._utilityService.getBannedCardName(bannedCard);
  }

  openTournamentSetupDialog() {
    this._modalService.open(TournamentSetupComponent);
  }

  kickContestantFromTournament(contestant: Contestant) {
    const cfrm = confirm('Really kick this player? ' + contestant.user.name);
    if (cfrm) {
      this._hubService.kickContestantFromTournament(contestant.user);
    }
  }

  userIsSpectator() {
    const exists = this.activeTournament.spectators.find(spectator => {
      return spectator.name === this.currentUser.name;
    });
    return exists != null;
  }

  openBanCardsDialog() {
    var banCardsModal = this._modalService.open(PickBannedCardsComponent);
    banCardsModal.componentInstance.bannedCards = Object.assign([], this.activeTournament.tournamentSetup.bannedCards);
    banCardsModal.result.then((bannedCards: CardValue[]) => {
      this.activeTournament.tournamentSetup.bannedCards = bannedCards;
      this.updateTournamentSetup();
    });
  }

  getGameTypePlaceholder() {
    return this.activeTournament.tournamentSetup.gameType == GameType.normal ? 'Normal' : 'Special Wild Cards';
  }

  joinTournament() {
    if (this.activeTournament.contestants.length >= this.activeTournament.tournamentSetup.numberOfPlayers) {
      this._toastrService.info(`Maximum number of players is reached ${this.activeTournament.tournamentSetup.numberOfPlayers}.`);
      return;
    }
    this._hubService.joinTournament(this.activeTournament.id, '');
  }

  startTournament() {
    if (this.activeTournament.contestants.length !== this.activeTournament.tournamentSetup.numberOfPlayers) {
      this._toastrService.info(`Not enough players. Tournament requires ${this.activeTournament.tournamentSetup.numberOfPlayers} players.`);
      return;
    }
    this._hubService.startTournament();
  }
  exitTournament() {
    this._router.navigateByUrl('/');
  }

  updateTournamentSetup() {
    this._hubService.updateTournamentSetup(this.activeTournament.id, this.activeTournament.tournamentSetup);
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
