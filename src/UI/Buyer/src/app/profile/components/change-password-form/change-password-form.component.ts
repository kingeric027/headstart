import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { MeUser } from '@ordercloud/angular-sdk';
import { ValidateStrongPassword, ValidateFieldMatches } from 'src/app/ocm-default-components/validators/validators';

@Component({
  selector: 'profile-change-password-form',
  templateUrl: './change-password-form.component.html',
  styleUrls: ['./change-password-form.component.scss'],
})
export class ChangePasswordFormComponent implements OnInit {
  form: FormGroup;
  me: MeUser;
  faTimes = faTimes;
  @Output()
  changePassword = new EventEmitter<{
    currentPassword: string;
    newPassword: string;
  }>();

  ngOnInit() {
    this.setForm();
  }

  setForm() {
    this.form = new FormGroup({
      currentPassword: new FormControl('', Validators.required),
      newPassword: new FormControl('', [Validators.required, ValidateStrongPassword]),
      confirmNewPassword: new FormControl('', ValidateFieldMatches('newPassword')),
    });
  }

  updatePassword() {
    if (this.form.status === 'INVALID') {
      return;
    }
    const { currentPassword, newPassword } = this.form.value;
    this.changePassword.emit({ currentPassword, newPassword });
    this.form.reset();
  }
}
