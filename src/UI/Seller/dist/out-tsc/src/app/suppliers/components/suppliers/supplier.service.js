import { __awaiter, __decorate, __extends, __generator, __metadata, __read, __spread } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcSupplierService, OcMeService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { MiddlewareAPIService } from '../middleware-api/middleware-api.service';
export var SUPPLIER_SUB_RESOURCE_LIST = ['users', 'locations'];
// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
var SupplierService = /** @class */ (function (_super) {
    __extends(SupplierService, _super);
    function SupplierService(router, activatedRoute, ocSupplierService, ocMeService, middleware) {
        var _this = _super.call(this, router, activatedRoute, ocSupplierService, '/suppliers', 'suppliers', SUPPLIER_SUB_RESOURCE_LIST) || this;
        _this.ocMeService = ocMeService;
        _this.middleware = middleware;
        _this.emptyResource = {
            Name: '',
            xp: {
                Description: '',
                Images: [{ URL: '', Tag: null }],
                SupportContact: { Name: '', Email: '', Phone: '' },
            },
        };
        _this.ocSupplierService = ocSupplierService;
        return _this;
    }
    SupplierService.prototype.createNewResource = function (resource) {
        return __awaiter(this, void 0, void 0, function () {
            var newSupplier;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.middleware.createSupplier(resource)];
                    case 1:
                        newSupplier = _a.sent();
                        this.resourceSubject.value.Items = __spread(this.resourceSubject.value.Items, [newSupplier]);
                        this.resourceSubject.next(this.resourceSubject.value);
                        return [2 /*return*/, newSupplier];
                }
            });
        });
    };
    SupplierService.prototype.getMyResource = function () {
        return __awaiter(this, void 0, void 0, function () {
            var me, supplier;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.ocMeService.Get().toPromise()];
                    case 1:
                        me = _a.sent();
                        return [4 /*yield*/, this.ocSupplierService.Get(me.Supplier.ID).toPromise()];
                    case 2:
                        supplier = _a.sent();
                        return [2 /*return*/, supplier];
                }
            });
        });
    };
    var _a;
    SupplierService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router,
            ActivatedRoute,
            OcSupplierService,
            OcMeService, typeof (_a = typeof MiddlewareAPIService !== "undefined" && MiddlewareAPIService) === "function" ? _a : Object])
    ], SupplierService);
    return SupplierService;
}(ResourceCrudService));
export { SupplierService };
//# sourceMappingURL=supplier.service.js.map