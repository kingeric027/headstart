// angular
import { Component, OnInit, OnChanges } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { ShopperContextService } from 'marketplace';
import { ToastrService } from 'ngx-toastr';

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class OCMLogin implements OnInit {
  form: FormGroup;
  isAnon: boolean;
  appName: string;
  ssoLink: string; // TODO - remove from marketplace generic. Should be SEB specific.

  constructor(private context: ShopperContextService, private toasterService: ToastrService) {}

  ngOnInit() {
    this.ssoLink = this.context.appSettings.ssoLink;
    this.appName = this.context.appSettings.appname;
    this.isAnon = this.context.currentUser.isAnonymous;
    this.form = new FormGroup({
      username: new FormControl(''),
      password: new FormControl(''),
      rememberMe: new FormControl(false),
    });
  }

  async onSubmit() {
    const username = this.form.get('username').value;
    const password = this.form.get('password').value;
    const rememberMe = this.form.get('rememberMe').value;
    try {
      await this.context.authentication.profiledLogin(username, password, rememberMe);
    } catch {
      this.toasterService.error(`Invalid Login Credentials`);
    }
  }

  showRegisterLink(): boolean {
    return this.isAnon && this.context.appSettings.anonymousShoppingEnabled;
  }
}
