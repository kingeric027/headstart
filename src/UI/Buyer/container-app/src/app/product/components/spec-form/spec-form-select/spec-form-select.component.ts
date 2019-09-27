import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';

@Component({
  selector: 'spec-form-select',
  template: `
    <div class="col-md-12">
      <div class="form-group" [formGroup]="group">
        <label for="{{config.name}}">{{ config.label }}</label>
        <select
          [formControlName]="config.name"
          class="form-control"
          value="{{config.value}}"
        >
          <option value=""></option>
          <option *ngFor="let option of config.options"> {{ option }} </option>
        </select>
      </div>
    </div>
  `,
  styleUrls: ['./spec-form-select.component.scss'],
})
export class SpecFormSelectComponent implements Field, OnInit {
  config: FieldConfig;
  group: FormGroup;
  index: number;

  constructor() {}

  ngOnInit() {}
}
