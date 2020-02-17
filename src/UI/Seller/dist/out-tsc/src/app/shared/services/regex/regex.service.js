import { __decorate, __metadata } from "tslib";
import { Injectable } from '@angular/core';
var RegexService = /** @class */ (function () {
    function RegexService() {
    }
    Object.defineProperty(RegexService.prototype, "ID", {
        // used for all Ordercloud IDs
        get: function () {
            return '^[a-zA-Z0-9_-]*$'; // only alphanumeric and _ -
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(RegexService.prototype, "ObjectName", {
        // used for ProductName, CategoryName
        get: function () {
            return '^[a-zA-Z0-9-(),:;&*\\s]{0,60}$'; // max 60 chars, alphanumeric, space and - ( ) , : ; & *
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(RegexService.prototype, "HumanName", {
        // used for FirstName, LastName, City
        get: function () {
            return "^[a-zA-Z0-9-.'\\s]*$"; // only alphanumic and space . '
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(RegexService.prototype, "Email", {
        get: function () {
            return '^.+@.+\\..+$'; // contains @ and . with text surrounding
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(RegexService.prototype, "Phone", {
        get: function () {
            return '^[0-9-]{0,20}$'; // max 20 chars, numbers and -
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(RegexService.prototype, "HundredChar", {
        // used for Carousel text
        get: function () {
            return '.{0,100}'; // max 100 chars
        },
        enumerable: true,
        configurable: true
    });
    RegexService.prototype.getZip = function (countryCode) {
        if (countryCode === void 0) { countryCode = 'US'; }
        switch (countryCode) {
            case 'CA':
                return '^[A-Za-z]\\d[A-Za-z][ -]?\\d[A-Za-z]\\d$'; // CA zip
            case 'US':
                return '^[0-9]{5}$'; // US zip - five numbers
        }
    };
    RegexService = __decorate([
        Injectable({
            providedIn: 'root',
        })
        // These regular expressions are all used for form validation
        ,
        __metadata("design:paramtypes", [])
    ], RegexService);
    return RegexService;
}());
export { RegexService };
//# sourceMappingURL=regex.service.js.map