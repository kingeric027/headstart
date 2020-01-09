import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MeUser } from '@ordercloud/angular-sdk';
import { ValidatePhone, ValidateName, ValidateEmail } from '../../../validators/validators';
import { ToastrService } from 'ngx-toastr';
import { ShopperContextService } from 'marketplace';
import { faEdit, faUser, faPhone, faEnvelope } from '@fortawesome/free-solid-svg-icons';
@Component({
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class OCMProfile implements OnInit {
  form: FormGroup;
  me: MeUser;
  alive = true;
  faEdit = faEdit;
  faUser = faUser;
  faPhone = faPhone;
  faEnvelope = faEnvelope;
  showEditProfileForm = false;

  constructor(private toasterService: ToastrService, private context: ShopperContextService) {}

  ngOnInit() {
    this.buildForm();
    this.context.currentUser.onUserChange(this.handleUserChange);
    console.log(this.form);
  }

  showEditProfile() {
    this.showEditProfileForm = true;
  }

  dismissProfileEditForm() {
    this.showEditProfileForm = false;
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

  async profileFormSubmitted(me: MeUser) {
    this.showEditProfileForm = false;
    await this.context.currentUser.patch(me);
    this.toasterService.success(`Profile Updated`);
  }
}
