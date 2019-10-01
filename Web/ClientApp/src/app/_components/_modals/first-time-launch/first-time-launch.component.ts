import { ModalService } from 'src/app/_services/modal-services/modal.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-first-time-launch',
  templateUrl: './first-time-launch.component.html',
  styleUrls: ['./first-time-launch.component.css']
})
export class FirstTimeLaunchComponent implements OnInit {
  sendToHelpPage: boolean = false;

  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit(): void {}

  updateAnswerSendToHelpPage(sendToHelpPage) {
    this.sendToHelpPage = sendToHelpPage;
    this._activeModal.close(this.sendToHelpPage);
  }
}
