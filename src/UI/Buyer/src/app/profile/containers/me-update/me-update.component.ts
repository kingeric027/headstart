import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { OcMeService, MeUser } from '@ordercloud/angular-sdk';
import { AppFormErrorService } from '@app-buyer/shared/services/form-error/form-error.service';
import { RegexService } from '@app-buyer/shared/services/regex/regex.service';
import { CurrentUserService } from '@app-buyer/shared/services/current-user/current-user.service';
import { IModalComponent } from '@app-buyer/shared/components/modal/modal.component';
import { AuthService } from '@app-buyer/shared/services/auth/auth.service';

@Component({
  selector: 'profile-meupdate',
  templateUrl: './me-update.component.html',
  styleUrls: ['./me-update.component.scss'],
})
export class MeUpdateComponent implements OnInit {
  form: FormGroup;
  me: MeUser;
  alive = true;
  @ViewChild('passwordModal', { static: false }) public passwordModal: IModalComponent;

  constructor(
    private currentUser: CurrentUserService,
    private formBuilder: FormBuilder,
    private formErrorService: AppFormErrorService,
    private authService: AuthService,
    private ocMeService: OcMeService,
    private toastrService: ToastrService,
    private regexService: RegexService
  ) {}

  ngOnInit() {
    this.setForm();
    this.getMeData();
  }

  private setForm() {
    this.form = this.formBuilder.group({
      Username: ['', Validators.required],
      FirstName: ['', [Validators.required, Validators.pattern(this.regexService.HumanName)]],
      LastName: ['', [Validators.required, Validators.pattern(this.regexService.HumanName)]],
      Email: ['', [Validators.required, Validators.email]],
      Phone: ['', Validators.pattern(this.regexService.Phone)],
    });
  }

  async onChangePassword({ currentPassword, newPassword }) {
    // todo - is this login check necessary?
    const creds = await this.authService.login(this.me.Username, currentPassword);
    if (!creds) return;
    await this.ocMeService.ResetPasswordByToken({ NewPassword: newPassword }).toPromise();
    this.toastrService.success('Account Info Updated', 'Success');
    this.passwordModal.close();
  }

  openPasswordModal() {
    this.passwordModal.open();
  }

  onSubmit() {
    if (this.form.status === 'INVALID') {
      return this.formErrorService.displayFormErrors(this.form);
    }

    const me = <MeUser>this.form.value;
    me.Active = true;

    this.ocMeService.Patch(me).subscribe((res) => {
      this.currentUser.user = res;
      this.toastrService.success('Account Info Updated');
    });
  }

  private getMeData() {
    this.ocMeService.Get().subscribe((me) => {
      this.me = me;
      this.form.setValue({
        Username: me.Username,
        FirstName: me.FirstName,
        LastName: me.LastName,
        Phone: me.Phone,
        Email: me.Email,
      });
    });
  }

  // control display of error messages
  hasRequiredError = (controlName: string): boolean => this.formErrorService.hasRequiredError(controlName, this.form);
  hasEmailError = (): boolean => this.formErrorService.hasInvalidEmailError(this.form.get('Email'));
  hasPatternError = (controlName: string) => this.formErrorService.hasPatternError(controlName, this.form);
  passwordMismatchError = (): boolean => this.formErrorService.hasPasswordMismatchError(this.form);
}
