import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'plural',
})
export class PluralPipe implements PipeTransform {
  transform(plural: string) {
    if (!plural) {
      return '';
    }
    return `${plural.toString().trim()}s`;
  }
}
