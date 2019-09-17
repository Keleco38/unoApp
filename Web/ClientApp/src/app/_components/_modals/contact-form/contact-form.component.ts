import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-contact-form',
  templateUrl: './contact-form.component.html',
  styleUrls: ['./contact-form.component.css']
})
export class ContactFormComponent implements OnInit {
  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  closeModal() {
    this._activeModal.dismiss();
  }

  sendForm() {
    this.closeModal();
  }
}
