import { Component, OnInit } from '@angular/core';
import { OCMComponent } from '../../base-component';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MeUser } from '@ordercloud/angular-sdk';
import { ValidatePhone, ValidateName, ValidateEmail } from '../../../validators/validators';
import { ToastrService } from 'ngx-toastr';
@Component({
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class OCMProfile extends OCMComponent implements OnInit {
  form: FormGroup;
  me: MeUser;
  alive = true;

  constructor(private toasterService: ToastrService) {
    super();
  }

  ngOnInit() {
    this.buildForm();
  }

  ngOnContextSet(): void {
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
  }

  private buildForm() {
    this.form = new FormGroup({
      Username: new FormControl('', Validators.required),
      FirstName: new FormControl('', [Validators.required, ValidateName]),
      LastName: new FormControl('', [Validators.required, ValidateName]),
      Email: new FormControl('', [Validators.required, ValidateEmail]),
      Phone: new FormControl('', ValidatePhone),
    });
  }

  async onSubmit() {
    const me: MeUser = this.form.value;
    me.Active = true;
    await this.context.currentUser.patch(me);
    this.toasterService.success(`Profile Updated`);
  }
}
