import { Component, OnInit, OnChanges } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { MeUser } from '@ordercloud/angular-sdk';
import { ValidateName, ValidatePhone, ValidateEmail } from 'src/app/ocm-default-components/validators/validators';
import { OCMComponent } from '../../shopper-context';

@Component({
  templateUrl: './me-update.component.html',
  styleUrls: ['./me-update.component.scss'],
})
export class OCMMeUpdateComponent extends OCMComponent implements OnInit, OnChanges {
  form: FormGroup;
  me: MeUser;
  alive = true;
  passwordModalOpen = false;

  ngOnInit() {
    this.buildForm();
  }

  ngOnChanges(): void {
    this.context.currentUser.onUserChange(this.handleUserChange);
  }

  handleUserChange = (user: MeUser) => {
    if (!user) return;
    this.me = user;
    this.form.setValue({
      Username: this.me.Username,
      FirstName: this.me.FirstName,
      LastName: this.me.LastName,
      Phone: this.me.Phone,
      Email: this.me.Email,
    });
  };

  private buildForm() {
    this.form = new FormGroup({
      Username: new FormControl('', Validators.required),
      FirstName: new FormControl('', [Validators.required, ValidateName]),
      LastName: new FormControl('', [Validators.required, ValidateName]),
      Email: new FormControl('', [Validators.required, ValidateEmail]),
      Phone: new FormControl('', ValidatePhone),
    });
  }

  async onChangePassword({ currentPassword, newPassword }) {
    await this.context.authentication.profiledLogin(this.me.Username, currentPassword, false);
    await this.context.authentication.changePassword(newPassword);
    this.passwordModalOpen = false;
  }

  openPasswordModal() {
    this.passwordModalOpen = true;
  }

  async onSubmit() {
    const me: MeUser = this.form.value;
    me.Active = true;
    await this.context.currentUser.patch(me);
  }
}
