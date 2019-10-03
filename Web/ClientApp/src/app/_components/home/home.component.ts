import { ModalService } from '../../_services/modal.service';
import { Component, OnInit ,OnDestroy} from '@angular/core';
import { HubService } from 'src/app/_services/hub.service';
import { environment } from 'src/environments/environment';
import { takeWhile } from 'rxjs/operators';
import { UserStorageService } from 'src/app/_services/storage-services/user-storage.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit,OnDestroy {
  private _isAlive=true;
  userReconnected: boolean;

  constructor(private _hubService: HubService, private _modalService: ModalService, private _userStorageService:UserStorageService) {}

  ngOnInit() {
    this._hubService.updateOnReconnect.pipe(takeWhile(() => this._isAlive)).subscribe(() => {
      this.userReconnected = true;
    });

    this._userStorageService.currentUser.pipe(takeWhile(() => this._isAlive)).subscribe((user) => {
      if(user){
        var name = localStorage.getItem('name');
        if (!environment.production) {
          const myArray = ['Ante', 'Mate', 'Jure', 'Ivica', 'John', 'Bruno', 'Mike', 'David', 'Mokki'];
          name = myArray[Math.floor(Math.random() * myArray.length)];
          localStorage.setItem('name', name);
        }
        this._hubService.addOrRenameUser(name);
      }
    });
  }

  removeAlert() {
    this.userReconnected = false;
  }

  createGame() {
    this._modalService.displayGameSetupModal();
  }

  ngOnDestroy(): void {
    this._isAlive=false;
  }
}
