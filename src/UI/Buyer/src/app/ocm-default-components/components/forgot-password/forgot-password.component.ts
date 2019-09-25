// angular
import { Component, OnInit, OnChanges, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

// ordercloud
import { OCMComponent } from '../../shopper-context';

@Component({
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
})
export class OCMForgotPassword extends OCMComponent implements OnInit, OnChanges {
  form: FormGroup;
  appName: string;
  @Output() forgotEvent = new EventEmitter<{ email: string }>();

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
