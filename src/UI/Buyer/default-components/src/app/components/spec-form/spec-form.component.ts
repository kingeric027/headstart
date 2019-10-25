import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormBuilder } from '@angular/forms';
import { FormGroup, Validators } from '@angular/forms';
import {
  map as _map,
  find as _find,
  minBy as _minBy,
  sortBy as _sortBy,
} from 'lodash';

import { FieldConfig } from './field-config.interface';
import { ListBuyerSpec, BuyerProduct, SpecOption } from '@ordercloud/angular-sdk';
import { SpecFormEvent } from './spec-form-values.interface';
import { OCMComponent } from '../base-component';

@Component({
  template: `
    <form *ngIf="form" [formGroup]="form">
      <ng-container *ngFor="let field of config; let i = index"
        formArrayName="ctrls"
        [config]="field"
        [group]="form"
        [index]="i"
        ocSpecField
      ></ng-container>
    </form>
    `,
  styleUrls: ['./spec-form.component.scss'],
})
export class OCMSpecForm extends OCMComponent {
  @Input() specs: ListBuyerSpec;
  @Input() product: BuyerProduct;
  @Input() qty: number;
  @Output() specFormChange: EventEmitter<any> = new EventEmitter<any>();

  config: FieldConfig[] = [];
  form: FormGroup;

  constructor(private fb: FormBuilder) {
    super();
  }

  ngOnContextSet() {
    this.config = this.createFieldConfig();
    this.form = this.createGroup();
    this.form.valueChanges.subscribe(() => {
      this.handleChange();
    });
    // trigger change to update referencing components form validity
    this.handleChange();
  }

  createGroup() {
    const group = this.fb.group({
      ctrls: this.fb.array([])
    });
    this.config.forEach((control) => {
      const ctrl = this.createControl(control);
      group.addControl(control.name, ctrl);
      // tslint:disable-next-line:no-string-literal
      group.controls['ctrls']['push'](ctrl);
    });
    return group;
  }

  createFieldConfig(): FieldConfig[] {
    const c: FieldConfig[] = [];
    if (!this.specs || !this.specs.Items) return c;
    for (const spec of this.specs.Items) {
      if (spec.xp.control === 'checkbox') {
        c.push({
          type: 'checkbox',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.DefaultOptionID,
          options: _map(spec.Options, 'Value'),
          validation: [Validators.nullValidator],
        });
      } else if (spec.xp.control === 'range') {
        c.push({
          type: 'range',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.DefaultValue,
          min: Math.min.apply(
            Math,
            _map(spec.Options, (option: SpecOption) => +option.Value)
          ),
          max: Math.max.apply(
            Math,
            _map(spec.Options, (option: SpecOption) => +option.Value)
          ),
          validation: [
            spec.Required ? Validators.required : Validators.nullValidator,
            Validators.min(
              Math.min.apply(
                Math,
                _map(spec.Options, (option: SpecOption) => +option.Value)
              )
            ),
            Validators.max(
              Math.max.apply(
                Math,
                _map(spec.Options, (option: SpecOption) => +option.Value)
              )
            ),
          ],
        });
      } else if (spec.Options.length === 1) {
        c.unshift({
          type: 'label',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          options: _map(spec.Options, 'Value'),
        });
      } else if (spec.Options.length > 1) {
        c.push({
          type: 'select',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.DefaultOptionID
            ? _find(spec.Options, (option) => {
                return option.ID === spec.DefaultOptionID;
              }).Value
            : null,
          options: _map(spec.Options, 'Value'),
          validation: [
            spec.Required ? Validators.required : Validators.nullValidator,
          ],
        });
      } else if (spec.AllowOpenText) {
        c.push({
          type: 'input',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.DefaultValue,
          validation: [
            spec.Required ? Validators.required : Validators.nullValidator,
          ],
        });
      }
    }
    return c;
  }

  createControl(config: FieldConfig): FormControl {
    const { disabled, validation, value } = config;
    return new FormControl({ disabled, value }, validation);
  }

  handleChange() {
    this.specFormChange.emit({
      type: 'Change',
      valid: this.form.valid,
      form: this.form.value
    } as SpecFormEvent);
  }
}
