// angular
import { Component, OnInit, OnChanges } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
})
export class OCMForgotPassword implements OnInit {
  form: FormGroup;
  appName: string;

  constructor(private context: ShopperContextService) {}

  ngOnInit() {
    this.appName = this.context.appSettings.appname;
    this.form = new FormGroup({ email: new FormControl('') });
  }

  async onSubmit() {
    const email = this.form.get('email').value;
    await this.context.authentication.forgotPasssword(email);
  }
}

