import { Pipe, PipeTransform } from '@angular/core';
import { MarketplaceMeProduct } from 'marketplace';
@Pipe({
  name: 'UofM',
})
export class UnitOfMeasurePipe implements PipeTransform {
  transform(product: MarketplaceMeProduct): string {
    const uofm = product?.xp?.UnitOfMeasure;  
    if (uofm?.Qty == null || uofm?.Unit == null) {
        return '';
    }
    return `${uofm.Qty} per ${uofm.Unit}`
  }
}
