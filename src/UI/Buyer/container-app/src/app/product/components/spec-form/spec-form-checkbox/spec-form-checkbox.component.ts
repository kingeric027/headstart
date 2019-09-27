import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';

@Component({
  selector: 'spec-form-checkbox',
  template: `
    <div class="col-md-12">
      <div class="form-group" [formGroup]="group">
        <input
          type="checkbox"
          class="form-control"
          [formControlName]="config.name"
        />{{ config.label }}
      </div>
    </div>
  `,
  styleUrls: ['./spec-form-checkbox.component.scss'],
})
export class SpecFormCheckboxComponent implements Field, OnInit {
  config: FieldConfig;
  group: FormGroup;
  index: number;

  constructor() {}

  ngOnInit() {}
}
