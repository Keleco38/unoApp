import { HubService } from 'src/app/_services/hub.service';
import { LobbyStorageService } from './../../../_services/storage-services/lobby-storage.service';
import { User } from 'src/app/_models/user';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-admin-section',
  templateUrl: './admin-section.component.html',
  styleUrls: ['./admin-section.component.css']
})
export class AdminSectionComponent implements OnInit, OnDestroy {
  private _isAlive = true;
  password: string;
  onlineUsers: User[];

  constructor(private _activeModal: NgbActiveModal, private _lobbyStorageService: LobbyStorageService, private _hubService:HubService) {}

  ngOnInit() {
    this._lobbyStorageService.onlineUsers.pipe(takeWhile(() => this._isAlive)).subscribe(onlinePlayers => {
      this.onlineUsers = onlinePlayers;
    });
  }

  closeModal() {
    this._activeModal.dismiss();
  }
  kickUserFromServer(user: User) {
    var cmfr = confirm(`Really kick ${user.name}?`);
    if (cmfr) {
      this._hubService.adminKickUser(user, this.password);
    }
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
