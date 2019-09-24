import { Component, OnInit, Input } from '@angular/core';
import { HttpService } from 'src/app/_services/http.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-rename',
  templateUrl: './rename.component.html',
  styleUrls: ['./rename.component.css']
})
export class RenameComponent implements OnInit {
  @Input('currentUser') currentUser: User = null;
  onlineUsers: string[];
  name: string = '';

  constructor(private _httpService: HttpService, private _activeModal: NgbActiveModal) {}

  ngOnInit(): void {
    this._httpService.getOnlineUsers().subscribe((onlineUsers: string[]) => {
      this.onlineUsers = onlineUsers;
      if (this.currentUser != null) {
        var index = this.onlineUsers.indexOf(this.currentUser.name);
        if (index != -1) this.onlineUsers.splice(index, 1);
      }
    });
  }

  processName() {
    this.name = this.name.toLocaleLowerCase().trim();
  }

  confirmUsername() {
    if (this.usernameTaken()) return;
    this._activeModal.close(this.name.toLowerCase());
  }

  usernameTaken() {
    if (!this.onlineUsers) return;
    if (this.onlineUsers.indexOf(this.name.toLowerCase()) != -1) return true;
    return false;
  }
}
