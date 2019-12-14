import { StickyTournament } from './../../_models/stickyTournament';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { takeWhile } from 'rxjs/operators';
import { HubService } from 'src/app/_services/hub.service';
import { LobbyStorageService } from 'src/app/_services/storage-services/lobby-storage.service';
import { ModalService } from 'src/app/_services/modal.service';

@Component({
  selector: 'app-sticky-tournaments',
  templateUrl: './sticky-tournaments.component.html',
  styleUrls: ['./sticky-tournaments.component.css']
})
export class StickyTournamentsComponent implements OnInit, OnDestroy {
  stickyTournaments: StickyTournament[] = [];
  _isAlive: boolean = true;

  constructor(
    private _lobbyStorageService: LobbyStorageService,
  ) { }

  ngOnInit() {
    this._lobbyStorageService.stickyTournaments.pipe(takeWhile(() => this._isAlive)).subscribe(tournaments => {
      this.stickyTournaments = tournaments;
    });
  }

  openStickyTournament(tournement: StickyTournament) {
    window.open(tournement.url, "_blank");
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }

}
