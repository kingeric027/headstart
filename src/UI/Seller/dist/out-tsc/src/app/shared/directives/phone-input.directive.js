import { __decorate, __metadata } from "tslib";
import { Directive, ElementRef, HostListener } from '@angular/core';
import { PhoneFormatPipe } from '@app-seller/shared/pipes/phone-format.pipe';
var PhoneInputDirective = /** @class */ (function () {
    function PhoneInputDirective(el, phoneFormat) {
        this.el = el;
        this.phoneFormat = phoneFormat;
    }
    PhoneInputDirective.prototype.keyUp = function () {
        this.format();
    };
    // Cap the length of the field at 14 characters - 10 numbers plus 4 characters
    PhoneInputDirective.prototype.keyDown = function (event) {
        var key = event.keyCode;
        if (!this.allowedKeys(key) && this.el.nativeElement.value.length === 14) {
            event.preventDefault();
        }
    };
    PhoneInputDirective.prototype.format = function () {
        this.el.nativeElement.value = this.phoneFormat.transform(this.el.nativeElement.value);
    };
    PhoneInputDirective.prototype.allowedKeys = function (key) {
        if (!key) {
            return true;
        }
        var isCtrlAltShift = 15 < key && key < 19;
        var isArrowKeys = 37 <= key && key <= 40;
        var isMetaKeys = key === 91;
        var isDelete = key === 46 || key === 8;
        var isTab = key === 9;
        return isCtrlAltShift || isArrowKeys || isMetaKeys || isDelete || isTab;
    };
    __decorate([
        HostListener('keyup'),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", void 0)
    ], PhoneInputDirective.prototype, "keyUp", null);
    __decorate([
        HostListener('keydown', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [KeyboardEvent]),
        __metadata("design:returntype", void 0)
    ], PhoneInputDirective.prototype, "keyDown", null);
    PhoneInputDirective = __decorate([
        Directive({ selector: '[appPhoneInput]' })
        // Directive class
        ,
        __metadata("design:paramtypes", [ElementRef, PhoneFormatPipe])
    ], PhoneInputDirective);
    return PhoneInputDirective;
}());
export { PhoneInputDirective };
//# sourceMappingURL=phone-input.directive.js.map