import { Component, OnInit } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { MeUser } from '@ordercloud/angular-sdk';
import { ValidateStrongPassword, ValidateFieldMatches } from '../../../validators/validators';
import { ToastrService } from 'ngx-toastr';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './change-password-form.component.html',
  styleUrls: ['./change-password-form.component.scss'],
})
export class OCMChangePasswordForm implements OnInit {
  form: FormGroup;
  me: MeUser;
  faTimes = faTimes;

  constructor(private toasterService: ToastrService, private context: ShopperContextService) {}

  ngOnInit() {
    this.setForm();
  }

  setForm() {
    this.form = new FormGroup({
      currentPassword: new FormControl('', Validators.required),
      newPassword: new FormControl('', [Validators.required, ValidateStrongPassword]),
      confirmNewPassword: new FormControl('', [ValidateFieldMatches('newPassword'), Validators.required]),
    });
  }

  resetFormValues() {
    this.form.controls.currentPassword.setValue('');
    this.form.controls.newPassword.setValue('');
    this.form.controls.confirmNewPassword.setValue('');
  }

  async updatePassword() {
    const { newPassword, currentPassword } = this.form.value;
    await this.context.authentication.validateCurrentPasswordAndChangePassword(newPassword, currentPassword);
    this.toasterService.success(`Password Changed`);
    this.resetFormValues();
  }
}
