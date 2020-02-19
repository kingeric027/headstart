import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';
import { specErrors } from '../errors';

@Component({
  selector: 'spec-form-label',
  template: `
    <div class="form-group" [formGroup]="group">
      <label class="text-uppercase font-weight-bolder small text-muted" for="{{ config.name }}">{{
        config.label
      }}</label>
      <p>{{ config.options[0] }}</p>
    </div>
  `,
  styleUrls: ['./spec-form-label.component.scss'],
})
export class SpecFormLabelComponent implements Field, OnInit {
  config: FieldConfig;
  group: FormGroup;
  index: number;
  errorMsgs = specErrors;

  constructor() {}

  ngOnInit(): void {}
}
