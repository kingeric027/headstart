// angular
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { ShopperContextService } from 'marketplace';
import { ToastrService } from 'ngx-toastr';

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class OCMLogin implements OnInit {
  form: FormGroup;
  appName: string;
  ssoLink: string; // TODO - remove from marketplace generic. Should be SEB specific.

  constructor(private context: ShopperContextService, private toasterService: ToastrService) {}

  ngOnInit(): void {
    this.ssoLink = this.context.appSettings.ssoLink;
    this.appName = this.context.appSettings.appname;
    this.form = new FormGroup({
      username: new FormControl(''),
      password: new FormControl(''),
      rememberMe: new FormControl(false),
    });
  }

  async onSubmit(): Promise<void> {
    const username = this.form.get('username').value;
    const password = this.form.get('password').value;
    const rememberMe = this.form.get('rememberMe').value;
    try {
      await this.context.authentication.profiledLogin(username, password, rememberMe);
    } catch {
      this.toasterService.error('Invalid Login Credentials');
    }
  }

  showRegisterLink(): boolean {
    return this.context.appSettings.anonymousShoppingEnabled;
  }
}
