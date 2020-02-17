import { __decorate, __extends, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcAddressService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { BUYER_SUB_RESOURCE_LIST } from './buyer.service';
// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
var BuyerLocationService = /** @class */ (function (_super) {
    __extends(BuyerLocationService, _super);
    function BuyerLocationService(router, activatedRoute, ocAddressService) {
        var _this = _super.call(this, router, activatedRoute, ocAddressService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'locations') || this;
        _this.emptyResource = {
            CompanyName: '',
            FirstName: '',
            LastName: '',
            Street1: '',
            Street2: '',
            City: '',
            State: '',
            Zip: '',
            Country: '',
            Phone: '',
            AddressName: 'Your new supplier location',
            xp: null,
        };
        return _this;
    }
    BuyerLocationService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router, ActivatedRoute, OcAddressService])
    ], BuyerLocationService);
    return BuyerLocationService;
}(ResourceCrudService));
export { BuyerLocationService };
//# sourceMappingURL=buyer-location.service.js.map