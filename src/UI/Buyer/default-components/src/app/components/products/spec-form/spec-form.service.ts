import { find as _find, sortBy as _sortBy } from 'lodash';
import { SpecOption, Spec, LineItemSpec, PriceBreak } from 'ordercloud-javascript-sdk';
import { Injectable } from '@angular/core';
import { Asset } from '@ordercloud/headstart-sdk';
import { FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class SpecFormService {

  constructor() { }

  public getSpecMarkup(specs: Spec[], selectedBreak: PriceBreak, qty: number, specForm: FormGroup): number {
    const formValues = specForm.value || undefined;
    const markups: Array<number> = new Array<number>();
    for (const value in formValues) {
      if (formValues.hasOwnProperty(value)) {
        const spec = this.getSpec(specs, value);
        if (!spec) continue;
        const option = this.getOption(spec, formValues[value]);
        if (option) {
          markups.push(this.singleSpecMarkup(selectedBreak.Price, qty, option));
        }
      }
    }
    return (selectedBreak.Price + markups.reduce((x, acc) => x + acc, 0)) * qty;
  }

  public getLineItemSpecs(buyerSpecs: Spec[], specForm: FormGroup): Array<LineItemSpec> {
    const formValues = specForm ? specForm.value : undefined;
    const specs: Array<LineItemSpec> = new Array<LineItemSpec>();
    for (const value in formValues) {
      if (formValues.hasOwnProperty(value)) {
        const spec = this.getSpec(buyerSpecs, value);
        if (!spec) continue;
        const option = this.getOption(spec, formValues[value]);
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

  public getGridLineItemSpecs(buyerSpecs: Spec[], specValues: string[]): GridSpecOption[] {
    const specs: GridSpecOption[] = [];
    for (let i = 0; i < buyerSpecs.length; i++) {
      const name = buyerSpecs[i].Name.replace(/ /g, '')
      const spec = this.getSpec(buyerSpecs, name);
      if (!spec) continue;
      const option = this.getOption(spec, specValues[i], 'grid');
      if (option) {
        specs.push({
          SpecID: spec.ID,
          OptionID: option.ID,
          Value: option.Value,
          MarkupType: option.PriceMarkupType,
          Markup: option.PriceMarkup
        });
      }
    }
    return specs;
  }

  public getLineItemImageUrl(images: Asset[], specs: Spec[], specForm: FormGroup): string {
    const image = images?.find(img => this.isImageMatchingSpecs(img, specs, specForm));
    return image?.Url;
  }

  private isImageMatchingSpecs(image: Asset, specs: Spec[], specForm: FormGroup): boolean {
    // Examine all specs, and find the image tag that matches all specs, removing spaces where needed on the spec to find that match.
    const liSpecs = this.getLineItemSpecs(specs, specForm);
    return liSpecs.
      every(spec => image.Tags
        .find(tag => tag?.split('-')
          .includes(spec.Value.replace(/\s/g, ''))));
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

  private getSpec(specs: Spec[], value: any): Spec {
    return _find(specs, item => item.Name.replace(/ /g, '') === value);
  }

  private getOption(spec: Spec, value: any, type?: string): SpecOption {
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
    if (type === 'grid') return _find(spec.Options, o => o.ID === value);
    else return _find(spec.Options, o => o.Value === value);
  }
}

export interface GridSpecOption {
  SpecID: string;
  OptionID: string;
  Value: string;
  MarkupType: string;
  Markup: number;
}
