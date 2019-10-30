import { Pipe, PipeTransform } from '@angular/core';
import { LineItem } from '@ordercloud/angular-sdk';

@Pipe({
  name: 'productNameWithSpecs',
})
export class ProductNameWithSpecsPipe implements PipeTransform {
  transform(lineItem: LineItem): any {
    // TODO - this check is needed because of the bare lineItem object that gets added right away.
    // If the activeProduct state was saved in some cache, this wouldn't be needed.
    if (!lineItem.Product) return ''; 
    const productName = lineItem.Product.Name;
    const specs = lineItem.Specs;
    if (specs.length === 0) return productName;
    const list = specs.map(spec => spec.Value).join(', ');
    return `${productName} (${list})`;
  }
}
