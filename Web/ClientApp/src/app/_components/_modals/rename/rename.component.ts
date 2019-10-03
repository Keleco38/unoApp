import { UserStorageService } from './../../../_services/storage-services/user-storage.service';
import { LobbyStorageService } from './../../../_services/storage-services/lobby-storage.service';
import { HubService } from './../../../_services/hub.service';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { HttpService } from 'src/app/_services/http.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { User } from 'src/app/_models/user';
import { pipe } from 'rxjs';
import { takeWhile, map } from 'rxjs/operators';

@Component({
  selector: 'app-rename',
  templateUrl: './rename.component.html',
  styleUrls: ['./rename.component.css']
})
export class RenameComponent implements OnInit, OnDestroy {
  private _isAlive = true;

  @Input('currentUser') currentUser: User = null;
  onlineUsers: string[] = [];
  name: string = '';

  constructor(
    private _hubService: HubService,
    private _lobbyStorageService: LobbyStorageService,
    private _userStorageService: UserStorageService,
    private _activeModal: NgbActiveModal
  ) {}

  ngOnInit(): void {
    this._lobbyStorageService.onlineUsers
      .pipe(takeWhile(() => this._isAlive))
      .pipe(
        map(users => {
          return users.map(user => {
            return user.name;
          });
        })
      )
      .subscribe((userNames: string[]) => {
        this.onlineUsers = userNames;
      });

    this._userStorageService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe(user => {
      var index = this.onlineUsers.indexOf(user.name);
      this.onlineUsers.splice(index, 1);
    });
  }

  processName() {
    this.name = this.name.toLocaleLowerCase().trim();
  }

  confirmUsername() {
    if (this.usernameTaken()) return;
    localStorage.setItem('name', this.name.toLocaleLowerCase());
    this._hubService.addOrRenameUser(this.name.toLowerCase());
    this._activeModal.dismiss();
  }

  usernameTaken() {
    if (this.onlineUsers.indexOf(this.name.toLowerCase()) != -1) return true;
    return false;
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
