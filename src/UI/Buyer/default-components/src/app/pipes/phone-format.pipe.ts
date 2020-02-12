import { Pipe, PipeTransform } from '@angular/core';
@Pipe({
    name: 'phoneFormat',
})
export class PhoneFormatPipe implements PipeTransform {
    transform(phoneNumber: string) {
        var cleaned = ('' + phoneNumber).replace(/\D/g, '')
        var match = cleaned.match(/^(1|)?(\d{3})(\d{3})(\d{4})$/)
        if (match) {
            var intlCode = (match[1] ? '+1 ' : '')
            return [intlCode, '(', match[2], ') ', match[3], '-', match[4]].join('')
        }
        return null
    }
}