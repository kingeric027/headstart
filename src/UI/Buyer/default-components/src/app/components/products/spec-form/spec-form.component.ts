import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormBuilder } from '@angular/forms';
import { FormGroup, Validators } from '@angular/forms';
import { map as _map, find as _find } from 'lodash';

import { FieldConfig } from './field-config.interface';
import { ListSpec, SpecOption } from '@ordercloud/angular-sdk';
import { SpecFormEvent } from './spec-form-values.interface';
import { exchange } from 'src/app/services/currency.helper';
import { ShopperContextService } from 'marketplace';

@Component({
  template: `
    <form *ngIf="form" [formGroup]="form">
      <ng-container
        *ngFor="let field of config; let i = index"
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
export class OCMSpecForm {
  _specs: ListSpec;
  @Output() specFormChange: EventEmitter<any> = new EventEmitter<any>();
  _myCurrency: string;
  config: FieldConfig[] = [];
  form: FormGroup;

  constructor(private fb: FormBuilder, private context: ShopperContextService) { }

  @Input() set specs(value: ListSpec) {
    const rates = this.context.exchangeRates.Get();
    this._myCurrency = rates.Items.find(r => r.Rate === 1 || r.Rate === 0).Currency;
    this._specs = value;
    // Exchange option markup prices based on currency of product/user
    this._specs.Items = this._specs.Items.map(s => {
      s.Options.map(o => {
        o.PriceMarkup = exchange(rates, o.PriceMarkup, this.currency).Price;
      })
      return s;
    })
    this.init();
  }

  @Input() currency: string;

  init(): void {
    this.config = this.createFieldConfig();
    this.form = this.createGroup();
    this.form.valueChanges.subscribe(() => {
      this.handleChange();
    });
    // trigger change to update referencing components form validity
    this.handleChange();
  }

  createGroup(): FormGroup {
    const group = this.fb.group({
      ctrls: this.fb.array([]),
    });
    this.config.forEach(control => {
      const ctrl = this.createControl(control);
      group.addControl(control.name, ctrl);
      // tslint:disable-next-line:no-string-literal
      (group.controls.ctrls as any).push(ctrl);
    });
    return group;
  }

  createFieldConfig(): FieldConfig[] {
    const c: FieldConfig[] = [];
    if (!this._specs || !this._specs.Items) return c;
    for (const spec of this._specs.Items) {
      if (spec?.xp?.control === 'checkbox') {
        c.push({
          type: 'checkbox',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.DefaultOptionID,
          options: _map(spec.Options, 'Value'),
          validation: [Validators.nullValidator],
        });
      } else if (spec?.xp?.control === 'range') {
        c.push({
          type: 'range',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.DefaultValue,
          min: Math.min(..._map(spec.Options, (option: SpecOption) => +option.Value)),
          max: Math.max(..._map(spec.Options, (option: SpecOption) => +option.Value)),
          validation: [
            spec.Required ? Validators.required : Validators.nullValidator,
            Validators.min(Math.min(..._map(spec.Options, (option: SpecOption) => +option.Value))),
            Validators.max(Math.max(..._map(spec.Options, (option: SpecOption) => +option.Value))),
          ],
        });
      } else if (spec?.Options.length === 1) {
        c.unshift({
          type: 'label',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          options: _map(spec.Options, 'Value'),
        });
      } else if (spec?.Options.length > 1) {
        c.push({
          type: 'select',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.DefaultOptionID
            ? _find(spec.Options, option => {
              return option.ID === spec.DefaultOptionID;
            }).Value
            : null,
          options: _map(spec.Options),
          validation: [spec.Required ? Validators.required : Validators.nullValidator],
          currency: this._myCurrency
        });
      } else if (spec.AllowOpenText) {
        c.push({
          type: 'input',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.DefaultValue,
          validation: [spec.Required ? Validators.required : Validators.nullValidator],
        });
      }
    }
    return c;
  }

  createControl(config: FieldConfig): FormControl {
    const { disabled, validation, value } = config;
    return new FormControl({ disabled, value }, validation);
  }

  handleChange(): void {
    this.specFormChange.emit({
      type: 'Change',
      valid: this.form.valid,
      form: this.form.value,
    } as SpecFormEvent);
  }
}
