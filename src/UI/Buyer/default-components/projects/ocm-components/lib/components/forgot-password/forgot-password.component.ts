// angular
import { Component, OnInit, OnChanges } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { OCMComponent } from '../base-component';


@Component({
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
})
export class OCMForgotPassword extends OCMComponent implements OnInit, OnChanges {
  form: FormGroup;
  appName: string;

  ngOnInit() {
    this.form = new FormGroup({ email: new FormControl('') });
  }

  ngOnChanges() {
    this.appName = this.context.appSettings.appname;
  }

  async onSubmit() {
    const email = this.form.get('email').value;
    await this.context.authentication.forgotPasssword(email);
  }
}
