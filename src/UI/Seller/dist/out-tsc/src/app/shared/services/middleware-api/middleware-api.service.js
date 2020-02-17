import { __awaiter, __decorate, __generator, __metadata, __param } from "tslib";
import { Injectable, Inject } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { DRAFT } from '@app-seller/shared/models/MarketPlaceProduct.interface';
var MiddlewareAPIService = /** @class */ (function () {
    function MiddlewareAPIService(ocTokenService, http, appConfig) {
        this.ocTokenService = ocTokenService;
        this.http = http;
        this.appConfig = appConfig;
        this.headers = {
            headers: new HttpHeaders({
                Authorization: "Bearer " + this.ocTokenService.GetAccess(),
            }),
        };
        this.baseUrl = this.appConfig.middlewareUrl;
        this.marketplaceID = this.appConfig.marketplaceID;
    }
    MiddlewareAPIService.prototype.getSuperMarketplaceProductByID = function (productID) {
        return __awaiter(this, void 0, void 0, function () {
            var url;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        url = this.baseUrl + "/products/" + productID;
                        return [4 /*yield*/, this.http.get(url, this.headers).toPromise()];
                    case 1: return [2 /*return*/, _a.sent()];
                }
            });
        });
    };
    MiddlewareAPIService.prototype.createNewSuperMarketplaceProduct = function (superMarketplaceProduct) {
        return __awaiter(this, void 0, void 0, function () {
            var url;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        superMarketplaceProduct.Product.xp.Status = DRAFT;
                        superMarketplaceProduct.PriceSchedule.Name = "Default_Marketplace_Buyer" + superMarketplaceProduct.Product.Name;
                        url = this.baseUrl + "/products";
                        return [4 /*yield*/, this.http.post(url, superMarketplaceProduct, this.headers).toPromise()];
                    case 1: return [2 /*return*/, _a.sent()];
                }
            });
        });
    };
    MiddlewareAPIService.prototype.updateMarketplaceProduct = function (superMarketplaceProduct) {
        return __awaiter(this, void 0, void 0, function () {
            var url;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        // TODO: Temporary while Product set doesn't reflect the current strongly typed Xp
                        superMarketplaceProduct.Product.xp.Status = DRAFT;
                        url = this.baseUrl + "/products/" + superMarketplaceProduct.Product.ID;
                        return [4 /*yield*/, this.http.put(url, superMarketplaceProduct, this.headers).toPromise()];
                    case 1: return [2 /*return*/, _a.sent()];
                }
            });
        });
    };
    MiddlewareAPIService.prototype.uploadProductImage = function (file, productID) {
        return __awaiter(this, void 0, void 0, function () {
            var url;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        url = this.baseUrl + "/" + this.marketplaceID + "/images/product/" + productID;
                        return [4 /*yield*/, this.http.post(url, this.formify(file), this.headers).toPromise()];
                    case 1: return [2 /*return*/, _a.sent()];
                }
            });
        });
    };
    MiddlewareAPIService.prototype.deleteProductImage = function (productID, imageUrl) {
        return __awaiter(this, void 0, void 0, function () {
            var imageName, url;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        imageName = imageUrl.split('/').slice(-1)[0];
                        url = this.baseUrl + "/" + this.marketplaceID + "/images/product/" + productID + "/" + imageName;
                        return [4 /*yield*/, this.http.delete(url, this.headers).toPromise()];
                    case 1: return [2 /*return*/, _a.sent()];
                }
            });
        });
    };
    MiddlewareAPIService.prototype.listTaxCodes = function (taxCategory, search, page, pageSize) {
        return __awaiter(this, void 0, void 0, function () {
            var url;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        url = this.baseUrl + "/taxcodes?taxCategory=" + taxCategory + "&search=" + search + "&pageSize=" + pageSize + "&page=" + page;
                        return [4 /*yield*/, this.http.get(url, this.headers).toPromise()];
                    case 1: return [2 /*return*/, _a.sent()];
                }
            });
        });
    };
    MiddlewareAPIService.prototype.createSupplier = function (supplier) {
        return __awaiter(this, void 0, void 0, function () {
            var url;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        url = this.baseUrl + "/supplier";
                        return [4 /*yield*/, this.http.post(url, supplier, this.headers).toPromise()];
                    case 1: return [2 /*return*/, _a.sent()];
                }
            });
        });
    };
    MiddlewareAPIService.prototype.formify = function (file) {
        var form = new FormData();
        form.append('file', file);
        return form;
    };
    MiddlewareAPIService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __param(2, Inject(applicationConfiguration)),
        __metadata("design:paramtypes", [OcTokenService,
            HttpClient, Object])
    ], MiddlewareAPIService);
    return MiddlewareAPIService;
}());
export { MiddlewareAPIService };
//# sourceMappingURL=middleware-api.service.js.map