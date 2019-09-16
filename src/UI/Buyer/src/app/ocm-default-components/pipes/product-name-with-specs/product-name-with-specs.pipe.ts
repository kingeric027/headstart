import { Pipe, PipeTransform } from '@angular/core';
import { LineItem } from '@ordercloud/angular-sdk';

@Pipe({
  name: 'productNameWithSpecs',
})
export class ProductNameWithSpecsPipe implements PipeTransform {
  transform(lineItem: LineItem): any {
    const productName = lineItem.Product.Name;
    const specs = lineItem.Specs;
    if (specs.length === 0) return productName;
    const list = specs.map((spec) => spec.Value).join(', ');
    return `${productName}(${list})`;
  }
}
