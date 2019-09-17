import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { OcMeService, OcTokenService, MeUser } from '@ordercloud/angular-sdk';
import { applicationConfiguration, AppConfig } from 'src/app/config/app.config';
import { AppFormErrorService } from 'src/app/shared/services/form-error/form-error.service';
import { AppMatchFieldsValidator } from 'src/app/shared/validators/match-fields/match-fields.validator';
import { RegexService } from 'src/app/shared/services/regex/regex.service';

@Component({
  selector: 'auth-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  form: FormGroup;
  me: MeUser;

  constructor(
    private formBuilder: FormBuilder,
    private formErrorService: AppFormErrorService,
    private ocMeService: OcMeService,
    private ocTokenService: OcTokenService,
    private router: Router,
    private toastrService: ToastrService,
    private regexService: RegexService,
    @Inject(applicationConfiguration) public appConfig: AppConfig
  ) {}

  ngOnInit() {
    this.setForm();
  }

  private setForm() {
    this.form = this.formBuilder.group(
      {
        Username: ['', Validators.required],
        FirstName: ['', [Validators.required, Validators.pattern(this.regexService.HumanName)]],
        LastName: ['', [Validators.required, Validators.pattern(this.regexService.HumanName)]],
        Email: ['', [Validators.required, Validators.email]],
        Phone: ['', Validators.pattern(this.regexService.Phone)],
        Password: ['', [Validators.required, Validators.minLength(8)]],
        ConfirmPassword: ['', [Validators.required, Validators.minLength(8)]],
      },
      {
        validator: AppMatchFieldsValidator('Password', 'ConfirmPassword'),
      }
    );
  }

  onSubmit() {
    if (this.form.status === 'INVALID') {
      return this.formErrorService.displayFormErrors(this.form);
    }

    const me = <MeUser>this.form.value;
    me.Active = true;

    this.ocMeService.Register(this.ocTokenService.GetAccess(), me).subscribe(() => {
      this.toastrService.success('New User Created');
      this.router.navigate(['/login']);
    });
  }

  // control display of error messages
  hasRequiredError = (controlName: string): boolean => this.formErrorService.hasRequiredError(controlName, this.form);
  hasEmailError = (): boolean => this.formErrorService.hasInvalidEmailError(this.form.get('Email'));
  hasPatternError = (controlName: string) => this.formErrorService.hasPatternError(controlName, this.form);
  passwordMismatchError = (): boolean => this.formErrorService.hasPasswordMismatchError(this.form);
}
