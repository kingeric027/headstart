import { __decorate, __extends, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcPromotionService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
var PromotionService = /** @class */ (function (_super) {
    __extends(PromotionService, _super);
    function PromotionService(router, activatedRoute, ocPromotionService) {
        return _super.call(this, router, activatedRoute, ocPromotionService, '/promotions', 'promotions') || this;
    }
    PromotionService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router, ActivatedRoute, OcPromotionService])
    ], PromotionService);
    return PromotionService;
}(ResourceCrudService));
export { PromotionService };
//# sourceMappingURL=promotion.service.js.map