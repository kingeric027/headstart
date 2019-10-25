import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';
import { specErrors } from '../errors';

@Component({
  selector: 'spec-form-label',
  template: `
    <div class="col-md-12" [formGroup]="group">
      {{ config.label }}: <b>{{ config.options[0] }}</b>
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

  ngOnInit() {
  }
}
