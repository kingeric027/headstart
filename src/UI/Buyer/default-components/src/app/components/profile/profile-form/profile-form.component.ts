import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
// 3rd party
import { MeUser } from '@ordercloud/angular-sdk';
import { ValidateName, ValidateEmail, ValidatePhone } from '../../../validators/validators';

@Component({
  templateUrl: './profile-form.component.html',
  styleUrls: ['./profile-form.component.scss'],
})
export class OCMProfileForm implements OnInit {
  private Me: MeUser = {};
  @Output() formDismissed = new EventEmitter();
  @Output()
  formSubmitted = new EventEmitter<{ me: MeUser }>();
  profileForm: FormGroup;

  ngOnInit() {
    this.setForm();
  }

  @Input() set me(me: MeUser) {
    this.Me = me || {};
    this.setForm();
    this.profileForm.markAsPristine();
  }

  setForm() {
    this.profileForm = new FormGroup({
      FirstName: new FormControl(this.Me.FirstName || '', [Validators.required, ValidateName]),
      LastName: new FormControl(this.Me.LastName || '', [Validators.required, ValidateName]),
      Username: new FormControl(this.Me.Username || '', Validators.required),
      Email: new FormControl(this.Me.Email || '', ValidateEmail),
      Phone: new FormControl(this.Me.Phone || '', ValidatePhone),
    });
  }

  onSubmit() {
    if (this.profileForm.status === 'INVALID') {
      return;
    }
    this.formSubmitted.emit({ me: this.profileForm.value });
  }

  dismissForm() {
    this.formDismissed.emit();
  }
}
