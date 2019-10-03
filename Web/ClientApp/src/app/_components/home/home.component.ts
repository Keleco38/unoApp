import { ModalService } from '../../_services/modal.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { environment } from 'src/environments/environment';
import { takeWhile } from 'rxjs/operators';
import { UserStorageService } from 'src/app/_services/storage-services/user-storage.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  private _isAlive = true;
  
  userReconnected: boolean;

  constructor(private _hubService: HubService, private _modalService: ModalService, private _userStorageService: UserStorageService) {}

  ngOnInit() {
    this._hubService.startConnection(false);
  }

  createGame() {
    this._modalService.displayGameSetupModal();
  }

  ngOnDestroy(): void {
    this._isAlive = false;
  }
}
