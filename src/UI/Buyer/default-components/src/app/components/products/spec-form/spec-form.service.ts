import { find as _find, sortBy as _sortBy } from 'lodash';
import { SpecFormEvent } from './spec-form-values.interface';
import { SpecOption, Spec, LineItemSpec, ListPage, PriceBreak } from 'ordercloud-javascript-sdk';
import { Injectable } from '@angular/core';
import { SuperMarketplaceProduct, AssetForDelivery } from '@ordercloud/headstart-sdk';
import { getPrimaryImageUrl } from 'src/app/services/images.helpers';

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

  public getSpecMarkup(specs: ListPage<Spec>, selectedBreak: PriceBreak, qty: number): number {
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

  public getLineItemSpecs(buyerSpecs: ListPage<Spec>): Array<LineItemSpec> {
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

  public getGridLineItemSpecs(buyerSpecs: ListPage<Spec>, specValues: string[]): GridSpecOption[] {
    const specs: GridSpecOption[] = [];
    for (let i = 0; i < buyerSpecs.Items.length; i++) {
      const name = buyerSpecs.Items[i].Name.replace(/ /g, '')
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

  public getLineItemImageUrl(product: SuperMarketplaceProduct): string {
    const image = product.Images?.find(img => this.isImageMatchingSpecs(img, product));
    return image?.Url;
  }

  private isImageMatchingSpecs(image: AssetForDelivery, product: SuperMarketplaceProduct): boolean {
    // Examine all specs, and find the image tag that matches all specs, removing spaces where needed on the spec to find that match.
  const specs = this.getLineItemSpecs({Meta: {}, Items: product.Specs as any});
  return specs.
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

  private getSpec(specs: ListPage<Spec>, value: any): Spec {
    return _find(specs?.Items, item => item.Name.replace(/ /g, '') === value);
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
