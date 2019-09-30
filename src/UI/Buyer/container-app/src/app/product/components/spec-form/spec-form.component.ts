import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { map as _map, find as _find, minBy as _minBy } from 'lodash';

import { FieldConfig } from './field-config.interface';
import { ListBuyerSpec, BuyerProduct, LineItemSpec, BuyerSpec, SpecOption } from '@ordercloud/angular-sdk';
import { SpecFormEvent } from '../../models/spec-form-values.interface';

@Component({
  selector: 'product-spec-form',
  template: `
    <form [formGroup]="form" (submit)="handleSubmit($event)">
      <ng-container
        *ngFor="let field of config; let i = index"
        class="my-1"
        formArrayName="ctrls"
        [config]="field"
        [group]="form"
        [index]="i"
        ocSpecField
      ></ng-container>
    </form>
  `,
})
export class SpecFormComponent implements OnInit {
  @Input() specs: ListBuyerSpec;
  @Input() product: BuyerProduct;
  @Output() submit: EventEmitter<any> = new EventEmitter<any>();
  @Output() change: EventEmitter<any> = new EventEmitter<any>();

  config: FieldConfig[] = [];
  form: FormGroup;

  get controls() {
    return this.config.filter(({ type }) => type !== 'button');
  }
  get changes() {
    return this.form.valueChanges;
  }
  get valid() {
    return this.form.valid;
  }
  get value() {
    return this.form.value;
  }

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.config = this.createFieldConfig();
    this.form = this.createGroup();
    this.form.valueChanges.subscribe(() => {
      this.handleChange();
    });
  }

  createFieldConfig(): FieldConfig[] {
    const c: FieldConfig[] = [];
    for (const spec of this.specs.Items) {
      if (spec.Options.length > 1) {
        c.push({
          type: 'select',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.DefaultOptionID,
          options: _map(spec.Options, 'Value'),
          validation: [spec.Required ? Validators.required : null],
        });
      } else if (spec.AllowOpenText) {
        c.push({
          type: 'input',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.DefaultValue,
          validation: [spec.Required ? Validators.required : null],
        });
      }
    }
    return c;
  }

  createGroup() {
    const group = this.fb.group({
      ctrls: this.fb.array([]),
    });
    this.config.forEach((control) => {
      const ctrl = this.createControl(control);
      group.addControl(control.name, ctrl);
      group.controls['ctrls']['push'](ctrl);
    });
    return group;
  }

  createControl(config: FieldConfig) {
    const { disabled, validation, value } = config;
    return this.fb.control({ disabled, value }, validation);
  }

  handleSubmit(event: Event) {
    event.preventDefault();
    event.stopPropagation();
    this.submit.emit({
      type: 'Submit',
      specs: this.getSpecs(),
      valid: this.valid,
      markup: this.getMarkup(),
    } as SpecFormEvent);
  }

  handleChange() {
    this.change.emit({
      type: 'Change',
      specs: this.getSpecs(),
      valid: this.valid,
      markup: this.getMarkup(),
    } as SpecFormEvent);
  }

  getMarkup(): number {
    // In OC, the price per item can depend on the quantity ordered. This info is stored on the PriceSchedule as a list of PriceBreaks.
    // Find the PriceBreak with the highest Quantity less than the quantity ordered. The price on that price break
    // is the cost per item.
    if (
      !this.product.PriceSchedule &&
      !this.product.PriceSchedule.PriceBreaks.length &&
      this.product.PriceSchedule.PriceBreaks[0].Price === 0
    ) {
      return 0;
    }
    const priceBreaks = this.product.PriceSchedule.PriceBreaks;
    const startingBreak = _minBy(priceBreaks, 'Quantity');

    const selectedBreak = priceBreaks.reduce((current, candidate) => {
      return candidate.Quantity > current.Quantity && candidate.Quantity <= this.value.quantity ? candidate : current;
    }, startingBreak);
    const markup = this.totalSpecMarkup(selectedBreak.Price);
    return (selectedBreak.Price + markup) * this.value.quantity;
  }

  totalSpecMarkup(unitPrice: number): number {
    const markups: Array<number> = new Array<number>();
    for (const value in this.value) {
      if (this.value.hasOwnProperty(value)) {
        const spec = this.getSpec(value);
        if (!spec) continue;
        const option = this.getOption(spec, this.value[value]);
        if (option) {
          markups.push(this.singleSpecMarkup(unitPrice, this.value.quantity, option));
        }
      }
    }
    return markups.reduce((x, acc) => x + acc, 0); //sum
  }

  singleSpecMarkup(unitPrice: number, quantity: number, option: SpecOption): number {
    switch (option.PriceMarkupType) {
      case 'NoMarkup':
        return 0;
      case 'AmountPerQuantity':
        return option.PriceMarkup;
      case 'AmountTotal':
        return option.PriceMarkup / quantity;
      case 'Percentage':
        return option.PriceMarkup * unitPrice * 0.01;
    }
  }

  getSpec(value: any): BuyerSpec {
    return _find(this.specs.Items, (item) => item.Name.replace(/ /g, '') === value) as BuyerSpec;
  }

  getSpecs(): Array<LineItemSpec> {
    const specs: Array<LineItemSpec> = new Array<LineItemSpec>();
    for (const value in this.value) {
      if (this.value.hasOwnProperty(value)) {
        const spec = this.getSpec(value);
        if (!spec) continue;
        const option = this.getOption(spec, this.value[value]);
        if (option) {
          specs.push({
            SpecID: spec.ID,
            OptionID: option.ID,
            Value: option.Value,
          });
        }
      }
    }
    return specs;
  }

  getOption(spec: BuyerSpec, value: any): SpecOption {
    if (typeof value === 'boolean') {
      return spec.Options[value ? 1 : 0] as SpecOption;
    }

    return _find(spec.Options, (o) => o.Value === value) as SpecOption;
  }

  setDisabled(name: string, disable: boolean) {
    if (this.form.controls[name]) {
      const method = disable ? 'disable' : 'enable';
      this.form.controls[name][method]();
      return;
    }

    this.config = this.config.map((item) => {
      if (item.name === name) {
        item.disabled = disable;
      }
      return item;
    });
  }

  setValue(name: string, value: any) {
    this.form.controls[name].setValue(value, { emitEvent: true });
  }
}
