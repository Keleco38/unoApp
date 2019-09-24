import { HttpService } from './../../../_services/http.service';
import { HubService } from 'src/app/_services/hub.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { takeWhile, map } from 'rxjs/operators';
import { RenameComponent } from '../rename/rename.component';

@Component({
  selector: 'app-first-time-launch',
  templateUrl: './first-time-launch.component.html',
  styleUrls: ['./first-time-launch.component.css']
})
export class FirstTimeLaunchComponent implements OnInit {
  sendToHelpPage: boolean = false;

  constructor(private _modalService: NgbModal, private _activeModal: NgbActiveModal) {}

  ngOnInit(): void {}

  updateAnswerSendToHelpPage(sendToHelpPage) {
    this.sendToHelpPage = sendToHelpPage;
    var modalRef = this._modalService.open(RenameComponent, { backdrop: 'static', keyboard: false });
    modalRef.result.then((name: string) => {
      localStorage.setItem('name', name);
      this._activeModal.close(this.sendToHelpPage);
    });
  }
}
