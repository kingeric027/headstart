import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { BuyerUserService } from '@app-seller/shared/services/buyer/buyer-user.service';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ValidateEmail } from '@app-seller/validators/validators';
function createBuyerUserForm(user) {
    return new FormGroup({
        Username: new FormControl(user.Username, Validators.required),
        FirstName: new FormControl(user.FirstName, Validators.required),
        LastName: new FormControl(user.LastName, Validators.required),
        Email: new FormControl(user.Email, [Validators.required, ValidateEmail]),
    });
}
var BuyerUserTableComponent = /** @class */ (function (_super) {
    __extends(BuyerUserTableComponent, _super);
    function BuyerUserTableComponent(buyerUserService, changeDetectorRef, router, activatedroute, buyerService, ngZone) {
        var _this = _super.call(this, changeDetectorRef, buyerUserService, router, activatedroute, ngZone, createBuyerUserForm) || this;
        _this.buyerUserService = buyerUserService;
        _this.buyerService = buyerService;
        return _this;
    }
    var _a, _b;
    BuyerUserTableComponent = __decorate([
        Component({
            selector: 'app-buyer-user-table',
            templateUrl: './buyer-user-table.component.html',
            styleUrls: ['./buyer-user-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof BuyerUserService !== "undefined" && BuyerUserService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute, typeof (_b = typeof BuyerService !== "undefined" && BuyerService) === "function" ? _b : Object, NgZone])
    ], BuyerUserTableComponent);
    return BuyerUserTableComponent;
}(ResourceCrudComponent));
export { BuyerUserTableComponent };
//# sourceMappingURL=buyer-user-table.component.js.map