import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { SellerUserService } from '@app-seller/shared/services/seller-user/seller-user.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ValidateEmail } from '@app-seller/validators/validators';
function createSellerUserForm(user) {
    return new FormGroup({
        Username: new FormControl(user.Username, Validators.required),
        FirstName: new FormControl(user.FirstName, Validators.required),
        LastName: new FormControl(user.LastName, Validators.required),
        Email: new FormControl(user.Email, [Validators.required, ValidateEmail]),
        Active: new FormControl(user.Active),
    });
}
var SellerUserTableComponent = /** @class */ (function (_super) {
    __extends(SellerUserTableComponent, _super);
    function SellerUserTableComponent(sellerUserService, changeDetectorRef, router, activatedroute, ngZone) {
        var _this = _super.call(this, changeDetectorRef, sellerUserService, router, activatedroute, ngZone, createSellerUserForm) || this;
        _this.sellerUserService = sellerUserService;
        return _this;
    }
    var _a;
    SellerUserTableComponent = __decorate([
        Component({
            selector: 'app-seller-user-table',
            templateUrl: './seller-user-table.component.html',
            styleUrls: ['./seller-user-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof SellerUserService !== "undefined" && SellerUserService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute,
            NgZone])
    ], SellerUserTableComponent);
    return SellerUserTableComponent;
}(ResourceCrudComponent));
export { SellerUserTableComponent };
//# sourceMappingURL=seller-user-table.component.js.map