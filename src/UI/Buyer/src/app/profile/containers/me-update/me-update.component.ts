import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { MeUser } from '@ordercloud/angular-sdk';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { ValidateName, ValidatePhone, ValidateEmail } from 'src/app/ocm-default-components/validators/validators';
import { ModalState } from 'src/app/ocm-default-components/models/modal-state.class';

@Component({
  selector: 'profile-meupdate',
  templateUrl: './me-update.component.html',
  styleUrls: ['./me-update.component.scss'],
})
export class MeUpdateComponent implements OnInit {
  form: FormGroup;
  me: MeUser;
  alive = true;
  passwordModal = ModalState.Closed;

  constructor(private toastrService: ToastrService, private context: ShopperContextService) {}

  ngOnInit() {
    this.buildForm();
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
    try {
      await this.context.authentication.profiledLogin(this.me.Username, currentPassword);
    } catch (ex) {
      this.toastrService.error('Current Password is incorrect');
    }
    await this.context.authentication.changePassword(newPassword);
    this.toastrService.success('Account Info Updated', 'Success');
    this.passwordModal = ModalState.Closed;
  }

  openPasswordModal() {
    this.passwordModal = ModalState.Open;
  }

  async onSubmit() {
    if (this.form.status === 'INVALID') {
      return;
    }

    const me: MeUser = this.form.value;
    me.Active = true;
    await this.context.currentUser.patch(me);
    this.toastrService.success('Account Info Updated');
  }
}
