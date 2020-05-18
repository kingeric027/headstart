import { find as _find, sortBy as _sortBy } from 'lodash';
import { SpecFormEvent } from './spec-form-values.interface';
import { PriceBreak, SpecOption, Spec, ListSpec, LineItemSpec } from '@ordercloud/angular-sdk';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SpecFormService {
  event: SpecFormEvent;

  constructor() {
    this.event = {
      valid: false,
      type: '',
      form: null,
    };
  }

  public getSpecMarkup(specs: ListSpec, selectedBreak: PriceBreak, qty: number): number {
    const markups: Array<number> = new Array<number>();
    for (const value in this.event.form) {
      if (this.event.form.hasOwnProperty(value)) {
        const spec = this.getSpec(specs, value);
        if (!spec) continue;
        const option = this.getOption(spec, this.event.form[value]);
        if (option) {
          markups.push(this.singleSpecMarkup(selectedBreak.Price, qty, option));
        }
      }
    }
    return (selectedBreak.Price + markups.reduce((x, acc) => x + acc, 0)) * qty;
  }

  public getLineItemSpecs(buyerSpecs: ListSpec): Array<LineItemSpec> {
    const specs: Array<LineItemSpec> = new Array<LineItemSpec>();
    for (const value in this.event.form) {
      if (this.event.form.hasOwnProperty(value)) {
        const spec = this.getSpec(buyerSpecs, value);
        if (!spec) continue;
        const option = this.getOption(spec, this.event.form[value]);
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

  private singleSpecMarkup(unitPrice: number, quantity: number, option: SpecOption): number {
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

  private getSpec(specs: ListSpec, value: any): Spec {
    console.log('specs', specs);
    return _find(specs?.Items, item => item.Name.replace(/ /g, '') === value);
  }

  private getOption(spec: Spec, value: any): SpecOption {
    if (value === undefined || value === null) {
      return null;
    }
    if (typeof value === 'boolean') {
      return spec.Options[value ? 1 : 0];
    }

    if (spec.xp && spec.xp.control === 'range') {
      const sorted = _sortBy(spec.Options, (v: SpecOption) => +v.Value);
      const o = _find(sorted, (option: SpecOption, index: number) => {
        if (sorted.length - 1 === index) {
          return sorted[index];
        } else if (+value === +option.Value || (+value > +option.Value && +value < +sorted[index + 1].Value)) {
          return sorted[index + 1];
        }
      });
      return o as SpecOption;
    }

    return _find(spec.Options, o => o.Value === value);
  }
}
