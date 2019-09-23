import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-contact-form',
  templateUrl: './contact-form.component.html',
  styleUrls: ['./contact-form.component.css']
})
export class ContactFormComponent implements OnInit {
  @ViewChild('contactForm', { static: true }) contactForm;

  subjectModel: string;
  emailModel: string;
  descriptionModel: string;
  formInvalid: boolean = false;
  formSubmitted: boolean = false;

  constructor(private _activeModal: NgbActiveModal) {}

  ngOnInit() {}

  closeModal() {
    this._activeModal.dismiss();
  }

  sendForm() {
    if (this.contactForm.form.invalid) {
      this.formInvalid = true;
    } else {
      this.contactForm.ngSubmit.emit();
      this.formSubmitted = true;
    }
  }
}
