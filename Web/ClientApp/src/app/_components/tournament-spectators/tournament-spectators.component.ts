import { UserStorageService } from 'src/app/_services/storage-services/user-storage.service';
import { ModalService } from './../../_services/modal.service';
import { TournamentStorageService } from './../../_services/storage-services/tournament-storage.service';
import { HubService } from 'src/app/_services/hub.service';
import { Tournament } from 'src/app/_models/tournament';
import { Component, OnInit } from '@angular/core';
import { takeWhile } from 'rxjs/operators';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-tournament-spectators',
  templateUrl: './tournament-spectators.component.html',
  styleUrls: ['./tournament-spectators.component.css']
})
export class TournamentSpectatorsComponent implements OnInit {
  private _isAlive: boolean = true;

  activeTournament: Tournament;
  currentUser: User;

  constructor(private _tournamentStorageService: TournamentStorageService, private _modalService: ModalService, private _userStorageService:UserStorageService) {}

  ngOnInit() {
    this._tournamentStorageService.activeTournament.pipe(takeWhile(() => this._isAlive)).subscribe(activeTournament => {
      this.activeTournament = activeTournament;
    });
    this._userStorageService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      this.currentUser = user;
    });
  }

  openKickBanUserModal(user: User) {
    this._modalService.displayKickBanPlayerModal(true, user);
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
