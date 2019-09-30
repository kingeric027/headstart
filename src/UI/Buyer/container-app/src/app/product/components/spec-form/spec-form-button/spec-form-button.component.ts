import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';

@Component({
  selector: 'spec-form-button',
  template: `
    <div class="form-group" [formGroup]="group">
      <div class="col-md-12">
        <button
          type="submit"
          class="btn btn-block btn-info"
          [disabled]="!group.valid"
        >
          <fa-icon [icon]="['fas', 'save']" size="md"></fa-icon>
          {{ config.label }}
        </button>
      </div>
    </div>
  `,
  styleUrls: ['./spec-form-button.component.scss'],
})
export class SpecFormButtonComponent implements Field, OnInit {
  config: FieldConfig;
  group: FormGroup;
  index: number;

  constructor() {}

  ngOnInit() {}
}
