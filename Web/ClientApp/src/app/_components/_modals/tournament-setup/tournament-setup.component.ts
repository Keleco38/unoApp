import { TournamentStorageService } from "./../../../_services/storage-services/tournament-storage.service";
import { TournamentSetup } from "./../../../_models/tournamentSetup";
import { Tournament } from "src/app/_models/tournament";
import { Component, OnInit } from "@angular/core";
import { HubService } from "../../../_services/hub.service";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { first } from "rxjs/operators";
import { PlayersSetup, GameType, CardValue } from "../../../_models/enums";
import { trigger, state, style, transition, animate } from "@angular/animations";

@Component({
  selector: "app-tournament-setup",
  templateUrl: "./tournament-setup.component.html",
  styleUrls: ["./tournament-setup.component.css"],
  animations: [
    trigger('expandCollapse', [
                state('open', style({opacity: 1})),
                state('closed', style({height: 0, opacity: 0})),
                transition('* => *', [animate('100ms')])
            ]),
 ]
})
export class TournamentSetupComponent implements OnInit {
  private _activeTournament: Tournament;

  tournamentSetup: TournamentSetup;
  hideAdvancedOptions=true;

  constructor(
    private _hubService: HubService,
    private _activeModal: NgbActiveModal,
    private _tournamentStorageService: TournamentStorageService
  ) { }

  ngOnInit() {
    this._tournamentStorageService.activeTournament
      .pipe(first())
      .subscribe((tournament: Tournament) => {
        this._activeTournament = JSON.parse(JSON.stringify(tournament));
      });

    if (this._activeTournament === null) {
      this.tournamentSetup = {
        roundsToWin: 5,
        numberOfPlayers: 32,
        reverseShouldSkipTurnInTwoPlayers: true,
        password: "",
        name: "",
        gameType: GameType.specialWildCards,
        drawFourDrawTwoShouldSkipTurn: true,
        bannedCards: [CardValue.swapHands, CardValue.paradigmShift, CardValue.magneticPolarity, CardValue.doubleDraw],
        matchingCardStealsTurn: true,
        spectatorsCanViewHands: true,
        wildCardCanBePlayedOnlyIfNoOtherOptions: false,
        drawAutoPlay: true,
        limitColorChangingCards: false,
        numberOfStandardDecks: 4
      };
    } else {
      this.tournamentSetup = this._activeTournament.tournamentSetup;
    }
  }

  closeModal() {
    this._activeModal.dismiss();
  }

  confirm() {
    if (this._activeTournament === null) {
      this._hubService.createTournament(
        this.tournamentSetup,
      );
    } else {
      this._hubService.updateTournamentSetup(this.tournamentSetup);
    }
    this._activeModal.close();
  }
}
