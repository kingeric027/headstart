import { __decorate } from "tslib";
import { Pipe } from '@angular/core';
var PhoneFormatPipe = /** @class */ (function () {
    function PhoneFormatPipe() {
    }
    PhoneFormatPipe.prototype.transform = function (tel) {
        if (!tel) {
            return '';
        }
        var value = tel
            .toString()
            .trim()
            .replace(/[^0-9]/g, '');
        var city, number;
        switch (value.length) {
            case 1:
            case 2:
            case 3:
                city = value;
                break;
            default:
                city = value.slice(0, 3);
                number = value.slice(3);
        }
        if (number) {
            if (number.length > 3) {
                number = number.slice(0, 3) + "-" + number.slice(3, 7);
            }
            return ("(" + city + ") " + number).trim();
        }
        else {
            return "(" + city;
        }
    };
    PhoneFormatPipe = __decorate([
        Pipe({
            name: 'tel',
        })
    ], PhoneFormatPipe);
    return PhoneFormatPipe;
}());
export { PhoneFormatPipe };
//# sourceMappingURL=phone-format.pipe.js.map