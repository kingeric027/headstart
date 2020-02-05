import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'asterisks',
    pure: false
})
export class AllAsterisksPipe implements PipeTransform {
    transform(value: string): any {
        return '*'.repeat(value.length);
    }
}
