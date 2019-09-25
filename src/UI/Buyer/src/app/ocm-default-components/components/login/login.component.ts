// angular
import { Component, OnInit, OnChanges } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

// ordercloud
import { OCMComponent } from 'src/app/ocm-default-components/shopper-context';

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class OCMLogin extends OCMComponent implements OnInit, OnChanges {
  form: FormGroup;
  isAnon: boolean;
  appName: string;

  ngOnInit() {
    this.form = new FormGroup({
      username: new FormControl(''),
      password: new FormControl(''),
      rememberMe: new FormControl(false),
    });
  }

  ngOnChanges() {
    this.appName = this.context.appSettings.appname;
    this.isAnon = this.context.currentUser.isAnonymous;
  }

  async onSubmit() {
    const username = this.form.get('username').value;
    const password = this.form.get('password').value;
    const rememberMe = this.form.get('rememberMe').value;
    await this.context.authentication.profiledLogin(username, password, rememberMe);
  }

  showRegisterLink(): boolean {
    return this.isAnon && this.context.appSettings.anonymousShoppingEnabled;
  }
}
