import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';
import { specErrors } from '../errors';

@Component({
  selector: 'spec-form-label',
  template: `
    <div class="form-group" [formGroup]="group" [class.row]="compact">
      <label class="text-uppercase font-weight-bolder small text-muted" [class.col-3]="compact" for="{{ config.name }}">{{
        config.label
      }}</label>
      <p [class.col-9]="compact">{{ config.options[0] }}</p>
    </div>
  `,
  styleUrls: ['./spec-form-label.component.scss'],
})
export class SpecFormLabelComponent implements Field, OnInit {
  config: FieldConfig;
  group: FormGroup;
  index: number;
  compact?: boolean;
  errorMsgs = specErrors;

  constructor() { }

  ngOnInit(): void { }
}
