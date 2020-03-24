import { Pipe, PipeTransform } from '@angular/core';
import { Product } from '../../../../marketplace/node_modules/@ordercloud/angular-sdk/dist/ordercloud-angular-sdk';
@Pipe({
  name: 'UofM',
})
export class UnitOfMeasurePipe implements PipeTransform {
  transform(product: Product): string {
    const uofm = product?.xp?.UnitOfMeasure;  
    if (uofm?.Qty == null || uofm?.Unit == null) {
        return '';
    }
    return `${uofm.Qty} / ${uofm.Unit}`
  }
}
