import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { OcMeService, MeUser } from '@ordercloud/angular-sdk';
import { AppMatchFieldsValidator } from 'src/app/shared/validators/match-fields/match-fields.validator';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { ValidateName, ValidatePhone, ValidateEmail, ValidateStrongPassword } from 'src/app/ocm-default-components/validators/validators';

@Component({
  selector: 'auth-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  form: FormGroup;
  me: MeUser;
  appName: string;

  constructor(
    private formBuilder: FormBuilder,
    private ocMeService: OcMeService,
    private toastrService: ToastrService,
    private context: ShopperContextService
  ) {}

  ngOnInit() {
    this.appName = this.context.appSettings.appname;
    this.setForm();
  }

  private setForm() {
    this.form = this.formBuilder.group(
      {
        Username: ['', Validators.required],
        FirstName: ['', [Validators.required, ValidateName]],
        LastName: ['', [Validators.required, ValidateName]],
        Email: ['', [Validators.required, ValidateEmail]],
        Phone: ['', ValidatePhone],
        Password: ['', [Validators.required, ValidateStrongPassword]],
        ConfirmPassword: ['', [Validators.required, ValidateStrongPassword]],
      },
      {
        validator: AppMatchFieldsValidator('Password', 'ConfirmPassword'),
      }
    );
  }

  onSubmit() {
    if (this.form.status === 'INVALID') {
      return;
    }

    const me = <MeUser>this.form.value;
    me.Active = true;
    const token = this.context.authentication.getOrderCloudToken();
    this.ocMeService.Register(token, me).subscribe(() => {
      this.toastrService.success('New User Created');
      this.context.routeActions.toLogin();
    });
  }
}
