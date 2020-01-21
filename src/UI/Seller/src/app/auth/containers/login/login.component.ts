// angular
import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';

@Component({
  selector: 'auth-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  form: FormGroup;
  isAnon: boolean;

  constructor(
    private currentUserService: CurrentUserService,
    private router: Router,
    private formBuilder: FormBuilder,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}

  ngOnInit() {
    this.form = this.formBuilder.group({
      username: '',
      password: '',
      rememberMe: false,
    });
  }

  async onSubmit() {
    await this.currentUserService.login(
      this.form.get('username').value,
      this.form.get('password').value,
      this.form.get('rememberMe').value
    );
    this.router.navigateByUrl('/home');
  }
}
