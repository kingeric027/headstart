import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { MeUser } from '@ordercloud/angular-sdk';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { ValidateName, ValidatePhone, ValidateEmail } from 'src/app/ocm-default-components/validators/validators';
import { ModalComponent } from 'src/app/shared/components/modal/modal.component';

@Component({
  selector: 'profile-meupdate',
  templateUrl: './me-update.component.html',
  styleUrls: ['./me-update.component.scss'],
})
export class MeUpdateComponent implements OnInit {
  form: FormGroup;
  me: MeUser;
  alive = true;
  @ViewChild('passwordModal', { static: false }) public passwordModal: ModalComponent;

  constructor(private formBuilder: FormBuilder, private toastrService: ToastrService, private context: ShopperContextService) {}

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
    this.form = this.formBuilder.group({
      Username: ['', Validators.required],
      FirstName: ['', [Validators.required, ValidateName]],
      LastName: ['', [Validators.required, ValidateName]],
      Email: ['', [Validators.required, ValidateEmail]],
      Phone: ['', ValidatePhone],
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
    this.passwordModal.close();
  }

  openPasswordModal() {
    this.passwordModal.open();
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
