import { __awaiter, __decorate, __extends, __generator, __metadata, __read, __spread } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcProductService, OcPriceScheduleService, OcCatalogService, } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
var ProductService = /** @class */ (function (_super) {
    __extends(ProductService, _super);
    function ProductService(router, activatedRoute, ocProductsService, ocPriceScheduleService, ocCatalogService) {
        var _this = _super.call(this, router, activatedRoute, ocProductsService, '/products', 'products') || this;
        _this.ocProductsService = ocProductsService;
        _this.ocPriceScheduleService = ocPriceScheduleService;
        _this.ocCatalogService = ocCatalogService;
        _this.emptyResource = {
            Product: {
                OwnerID: '',
                DefaultPriceScheduleID: '',
                AutoForward: false,
                Active: false,
                ID: null,
                Name: null,
                Description: null,
                QuantityMultiplier: null,
                ShipWeight: null,
                ShipHeight: null,
                ShipWidth: null,
                ShipLength: null,
                ShipFromAddressID: null,
                Inventory: null,
                DefaultSupplierID: null,
                xp: {
                    IntegrationData: null,
                    Facets: null,
                    Images: [],
                    Status: null,
                    HasVariants: false,
                    Note: '',
                    Tax: {
                        Category: null,
                        Code: null,
                        Description: null,
                    },
                    UnitOfMeasure: null,
                },
            },
            PriceSchedule: {
                ID: '',
                Name: '',
                ApplyTax: false,
                ApplyShipping: false,
                MinQuantity: 1,
                MaxQuantity: null,
                UseCumulativeQuantity: false,
                RestrictedQuantity: false,
                PriceBreaks: [
                    {
                        Quantity: 1,
                        Price: null,
                    },
                ],
                xp: {},
            },
        };
        return _this;
    }
    ProductService.prototype.updateProductCatalogAssignments = function (add, del) {
        return __awaiter(this, void 0, void 0, function () {
            var addRequests, deleteRequests;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        addRequests = add.map(function (newAssignment) { return _this.addProductCatalogAssignment(newAssignment); });
                        deleteRequests = del.map(function (assignmentToRemove) { return _this.removeProductCatalogAssignment(assignmentToRemove); });
                        return [4 /*yield*/, Promise.all(__spread(addRequests, deleteRequests))];
                    case 1:
                        _a.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductService.prototype.addProductCatalogAssignment = function (assignment) {
        return this.ocCatalogService
            .SaveProductAssignment({ CatalogID: assignment.CatalogID, ProductID: assignment.ProductID })
            .toPromise();
    };
    ProductService.prototype.removeProductCatalogAssignment = function (assignment) {
        return this.ocCatalogService.DeleteProductAssignment(assignment.CatalogID, assignment.ProductID).toPromise();
    };
    ProductService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router,
            ActivatedRoute,
            OcProductService,
            OcPriceScheduleService,
            OcCatalogService])
    ], ProductService);
    return ProductService;
}(ResourceCrudService));
export { ProductService };
//# sourceMappingURL=product.service.js.map