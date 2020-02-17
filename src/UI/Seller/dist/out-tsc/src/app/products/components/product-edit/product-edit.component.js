import { __assign, __awaiter, __decorate, __generator, __metadata, __param, __read, __spread, __values } from "tslib";
import { Component, Input, Output, EventEmitter, Inject, ChangeDetectorRef } from '@angular/core';
import { get as _get } from 'lodash';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { OcSupplierAddressService, OcAdminAddressService, OcProductService, } from '@ordercloud/angular-sdk';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { DomSanitizer } from '@angular/platform-browser';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { ReplaceHostUrls } from '@app-seller/shared/services/product/product-image.helper';
import { faTrash, faTimes } from '@fortawesome/free-solid-svg-icons';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
var ProductEditComponent = /** @class */ (function () {
    function ProductEditComponent(changeDetectorRef, router, currentUserService, ocSupplierAddressService, ocProductService, ocAdminAddressService, productService, middleware, sanitizer, modalService, toasterService, appConfig) {
        this.changeDetectorRef = changeDetectorRef;
        this.router = router;
        this.currentUserService = currentUserService;
        this.ocSupplierAddressService = ocSupplierAddressService;
        this.ocProductService = ocProductService;
        this.ocAdminAddressService = ocAdminAddressService;
        this.productService = productService;
        this.middleware = middleware;
        this.sanitizer = sanitizer;
        this.modalService = modalService;
        this.toasterService = toasterService;
        this.appConfig = appConfig;
        this.updateResource = new EventEmitter();
        this.dataIsSaving = false;
        this.userContext = {};
        this.hasVariations = false;
        this.images = [];
        this.files = [];
        this.faTrash = faTrash;
        this.faTimes = faTimes;
        this.areChanges = false;
        this.taxCodeCategorySelected = false;
    }
    Object.defineProperty(ProductEditComponent.prototype, "orderCloudProduct", {
        set: function (product) {
            if (product.ID) {
                this.handleSelectedProductChange(product);
            }
            else {
                this.createProductForm(this.productService.emptyResource);
            }
        },
        enumerable: true,
        configurable: true
    });
    ProductEditComponent.prototype.ngOnInit = function () {
        return __awaiter(this, void 0, void 0, function () {
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        // TODO: Eventually move to a resolve so that they are there before the component instantiates.
                        this.checkIfCreatingNew();
                        this.getAddresses();
                        _a = this;
                        return [4 /*yield*/, this.currentUserService.getUserContext()];
                    case 1:
                        _a.userContext = _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.getAddresses = function () {
        return __awaiter(this, void 0, void 0, function () {
            var context, _a, _b, _c;
            return __generator(this, function (_d) {
                switch (_d.label) {
                    case 0: return [4 /*yield*/, this.currentUserService.getUserContext()];
                    case 1:
                        context = _d.sent();
                        if (!context.Me.Supplier) return [3 /*break*/, 3];
                        _b = this;
                        return [4 /*yield*/, this.ocSupplierAddressService.List(context.Me.Supplier.ID).toPromise()];
                    case 2:
                        _a = (_b.addresses = _d.sent());
                        return [3 /*break*/, 5];
                    case 3:
                        _c = this;
                        return [4 /*yield*/, this.ocAdminAddressService.List().toPromise()];
                    case 4:
                        _a = (_c.addresses = _d.sent());
                        _d.label = 5;
                    case 5:
                        _a;
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.handleSelectedProductChange = function (product) {
        return __awaiter(this, void 0, void 0, function () {
            var marketPlaceProduct;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.middleware.getSuperMarketplaceProductByID(product.ID)];
                    case 1:
                        marketPlaceProduct = _a.sent();
                        this.refreshProductData(marketPlaceProduct);
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.refreshProductData = function (superProduct) {
        return __awaiter(this, void 0, void 0, function () {
            var taxCategory, avalaraTaxCodes;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this._superMarketplaceProductStatic = superProduct;
                        this._superMarketplaceProductEditable = superProduct;
                        if (!(this._superMarketplaceProductEditable.Product &&
                            this._superMarketplaceProductEditable.Product.xp &&
                            this._superMarketplaceProductEditable.Product.xp.Tax &&
                            this._superMarketplaceProductEditable.Product.xp.Tax.Category)) return [3 /*break*/, 2];
                        taxCategory = this._superMarketplaceProductEditable.Product.xp.Tax.Category === 'FR000000'
                            ? this._superMarketplaceProductEditable.Product.xp.Tax.Category.substr(0, 2)
                            : this._superMarketplaceProductEditable.Product.xp.Tax.Category.substr(0, 1);
                        return [4 /*yield*/, this.middleware.listTaxCodes(taxCategory, '', 1, 100)];
                    case 1:
                        avalaraTaxCodes = _a.sent();
                        this.taxCodes = avalaraTaxCodes;
                        return [3 /*break*/, 3];
                    case 2:
                        this.taxCodes = { Meta: {}, Items: [] };
                        _a.label = 3;
                    case 3:
                        this.createProductForm(superProduct);
                        this.images = ReplaceHostUrls(superProduct.Product);
                        this.taxCodeCategorySelected =
                            (this._superMarketplaceProductEditable.Product &&
                                this._superMarketplaceProductEditable.Product.xp &&
                                this._superMarketplaceProductEditable.Product.xp.Tax &&
                                this._superMarketplaceProductEditable.Product.xp.Tax.Category) !== null;
                        this.checkIfCreatingNew();
                        this.checkForChanges();
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.checkIfCreatingNew = function () {
        var routeUrl = this.router.routerState.snapshot.url;
        var endUrl = routeUrl.slice(routeUrl.length - 4, routeUrl.length);
        this.isCreatingNew = endUrl === '/new';
    };
    ProductEditComponent.prototype.createProductForm = function (superMarketplaceProduct) {
        this.productForm = new FormGroup({
            Name: new FormControl(superMarketplaceProduct.Product.Name, [Validators.required, Validators.maxLength(100)]),
            ID: new FormControl(superMarketplaceProduct.Product.ID),
            Description: new FormControl(superMarketplaceProduct.Product.Description, Validators.maxLength(1000)),
            Inventory: new FormControl(superMarketplaceProduct.Product.Inventory),
            QuantityMultiplier: new FormControl(superMarketplaceProduct.Product.QuantityMultiplier),
            ShipFromAddressID: new FormControl(superMarketplaceProduct.Product.ShipFromAddressID),
            ShipHeight: new FormControl(superMarketplaceProduct.Product.ShipHeight, [Validators.required, Validators.min(0)]),
            ShipWidth: new FormControl(superMarketplaceProduct.Product.ShipWidth, [Validators.required, Validators.min(0)]),
            ShipLength: new FormControl(superMarketplaceProduct.Product.ShipLength, [Validators.required, Validators.min(0)]),
            ShipWeight: new FormControl(superMarketplaceProduct.Product.ShipWeight, [Validators.required, Validators.min(0)]),
            Price: new FormControl(_get(superMarketplaceProduct.PriceSchedule, 'PriceBreaks[0].Price', null)),
            Note: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Note'), Validators.maxLength(140)),
            // SpecCount: new FormControl(superMarketplaceProduct.SpecCount),
            // VariantCount: new FormControl(superMarketplaceProduct.VariantCount),
            TaxCodeCategory: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Tax.Category', null)),
            TaxCode: new FormControl(_get(superMarketplaceProduct.Product, 'xp.Tax.Code', null)),
        });
    };
    ProductEditComponent.prototype.handleSave = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!this.isCreatingNew) return [3 /*break*/, 2];
                        return [4 /*yield*/, this.createNewProduct()];
                    case 1:
                        _a.sent();
                        return [3 /*break*/, 3];
                    case 2:
                        this.updateProduct();
                        _a.label = 3;
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.handleDelete = function ($event) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.ocProductService.Delete(this._superMarketplaceProductStatic.Product.ID).toPromise()];
                    case 1:
                        _a.sent();
                        this.router.navigateByUrl('/products');
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.handleDiscardChanges = function () {
        this.files = [];
        this._superMarketplaceProductEditable = this._superMarketplaceProductStatic;
        this.refreshProductData(this._superMarketplaceProductStatic);
    };
    ProductEditComponent.prototype.createNewProduct = function () {
        return __awaiter(this, void 0, void 0, function () {
            var superProduct, ex_1;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 3, , 4]);
                        this.dataIsSaving = true;
                        return [4 /*yield*/, this.middleware.createNewSuperMarketplaceProduct(this._superMarketplaceProductEditable)];
                    case 1:
                        superProduct = _a.sent();
                        return [4 /*yield*/, this.addFiles(this.files, superProduct.Product.ID)];
                    case 2:
                        _a.sent();
                        this.refreshProductData(superProduct);
                        this.router.navigateByUrl("/products/" + superProduct.Product.ID);
                        this.dataIsSaving = false;
                        return [3 /*break*/, 4];
                    case 3:
                        ex_1 = _a.sent();
                        this.dataIsSaving = false;
                        if (ex_1.error && ex_1.error.Errors && ex_1.error.Errors.some(function (e) { return e.ErrorCode === 'IdExists'; })) {
                            this.toasterService.error("A product with that ID already exists");
                        }
                        else {
                            throw ex_1;
                        }
                        return [3 /*break*/, 4];
                    case 4: return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.updateProduct = function () {
        return __awaiter(this, void 0, void 0, function () {
            var superProduct, ex_2;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        this.dataIsSaving = true;
                        return [4 /*yield*/, this.middleware.updateMarketplaceProduct(this._superMarketplaceProductEditable)];
                    case 1:
                        superProduct = _a.sent();
                        this._superMarketplaceProductStatic = superProduct;
                        this._superMarketplaceProductEditable = superProduct;
                        if (this.files)
                            this.addFiles(this.files, superProduct.Product.ID);
                        this.dataIsSaving = false;
                        return [3 /*break*/, 3];
                    case 2:
                        ex_2 = _a.sent();
                        this.dataIsSaving = false;
                        throw ex_2;
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.updateProductResource = function (productUpdate) {
        /*
        * TODO:
        * This function is used to dynamically update deeply nested objects
        * It is currently used in two places, but will likely soon become
        * obsolete when the product edit component gets refactored.
        */
        var piecesOfField = productUpdate.field.split('.');
        var depthOfField = piecesOfField.length;
        var updateProductResourceCopy = this.copyProductResource(this._superMarketplaceProductEditable || this.productService.emptyResource);
        console.log(updateProductResourceCopy);
        switch (depthOfField) {
            case 4:
                updateProductResourceCopy[piecesOfField[0]][piecesOfField[1]][piecesOfField[2]][piecesOfField[3]] =
                    productUpdate.value;
                break;
            case 3:
                updateProductResourceCopy[piecesOfField[0]][piecesOfField[1]][piecesOfField[2]] = productUpdate.value;
                break;
            case 2:
                updateProductResourceCopy[piecesOfField[0]][piecesOfField[1]] = productUpdate.value;
                break;
            default:
                updateProductResourceCopy[piecesOfField[0]] = productUpdate.value;
                break;
        }
        this._superMarketplaceProductEditable = updateProductResourceCopy;
        this.checkForChanges();
    };
    ProductEditComponent.prototype.handleUpdateProduct = function (event, field, typeOfValue) {
        var productUpdate = {
            field: field,
            value: field === 'Active'
                ? event.target.checked
                : typeOfValue === 'number'
                    ? Number(event.target.value)
                    : event.target.value,
        };
        this.updateProductResource(productUpdate);
    };
    ProductEditComponent.prototype.copyProductResource = function (product) {
        return JSON.parse(JSON.stringify(product));
    };
    // Used only for Product.Description coming out of quill editor (no 'event.target'.)
    ProductEditComponent.prototype.updateResourceFromFieldValue = function (field, value) {
        var _a;
        var updateProductResourceCopy = this.copyProductResource(this._superMarketplaceProductEditable || this.productService.emptyResource);
        updateProductResourceCopy.Product = __assign(__assign({}, updateProductResourceCopy.Product), (_a = {}, _a[field] = value, _a));
        this._superMarketplaceProductEditable = updateProductResourceCopy;
        this.checkForChanges();
    };
    ProductEditComponent.prototype.checkForChanges = function () {
        this.areChanges =
            JSON.stringify(this._superMarketplaceProductEditable) !== JSON.stringify(this._superMarketplaceProductStatic) ||
                this.files.length > 0;
    };
    /******************************************
     *  **** PRODUCT IMAGE UPLOAD FUNCTIONS ****
     * ******************************************/
    ProductEditComponent.prototype.manualFileUpload = function (event) {
        var _this = this;
        var files = Array.from(event.target.files).map(function (file) {
            var URL = _this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
            return { File: file, URL: URL };
        });
        this.stageFiles(files);
    };
    ProductEditComponent.prototype.stageFiles = function (files) {
        this.files = this.files.concat(files);
        this.checkForChanges();
    };
    ProductEditComponent.prototype.addFiles = function (files, productID) {
        return __awaiter(this, void 0, void 0, function () {
            var superProduct, files_1, files_1_1, file, e_1_1;
            var e_1, _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        _b.trys.push([0, 5, 6, 7]);
                        files_1 = __values(files), files_1_1 = files_1.next();
                        _b.label = 1;
                    case 1:
                        if (!!files_1_1.done) return [3 /*break*/, 4];
                        file = files_1_1.value;
                        return [4 /*yield*/, this.middleware.uploadProductImage(file.File, productID)];
                    case 2:
                        superProduct = _b.sent();
                        _b.label = 3;
                    case 3:
                        files_1_1 = files_1.next();
                        return [3 /*break*/, 1];
                    case 4: return [3 /*break*/, 7];
                    case 5:
                        e_1_1 = _b.sent();
                        e_1 = { error: e_1_1 };
                        return [3 /*break*/, 7];
                    case 6:
                        try {
                            if (files_1_1 && !files_1_1.done && (_a = files_1.return)) _a.call(files_1);
                        }
                        finally { if (e_1) throw e_1.error; }
                        return [7 /*endfinally*/];
                    case 7:
                        this.files = [];
                        // Only need the `|| {}` to account for creating new product where this._superMarketplaceProductStatic doesn't exist yet.
                        superProduct = Object.assign(this._superMarketplaceProductStatic || {}, superProduct);
                        this.refreshProductData(superProduct);
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.removeFile = function (imgUrl) {
        return __awaiter(this, void 0, void 0, function () {
            var superProduct;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.middleware.deleteProductImage(this._superMarketplaceProductStatic.Product.ID, imgUrl)];
                    case 1:
                        superProduct = _a.sent();
                        superProduct = Object.assign(this._superMarketplaceProductStatic, superProduct);
                        this.refreshProductData(superProduct);
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.unStage = function (index) {
        this.files.splice(index, 1);
        this.checkForChanges();
    };
    ProductEditComponent.prototype.open = function (content) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.modalService.open(content, { ariaLabelledBy: 'confirm-modal' })];
                    case 1:
                        _a.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.handleTaxCodeCategorySelection = function (event) {
        return __awaiter(this, void 0, void 0, function () {
            var avalaraTaxCodes;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        // TODO: This is a temporary fix to accomodate for data not having xp.TaxCode yet
                        if (this._superMarketplaceProductEditable.Product &&
                            this._superMarketplaceProductEditable.Product.xp &&
                            !this._superMarketplaceProductEditable.Product.xp.Tax) {
                            this._superMarketplaceProductEditable.Product.xp.Tax = { Category: '', Code: '', Description: '' };
                        }
                        this.resetTaxCodeAndDescription();
                        this.handleUpdateProduct(event, 'Product.xp.Tax.Category');
                        this._superMarketplaceProductEditable.Product.xp.Tax.Code = '';
                        return [4 /*yield*/, this.middleware.listTaxCodes(event.target.value, '', 1, 100)];
                    case 1:
                        avalaraTaxCodes = _a.sent();
                        this.taxCodes = avalaraTaxCodes;
                        return [2 /*return*/];
                }
            });
        });
    };
    // Reset TaxCode Code and Description if a new TaxCode Category is selected
    ProductEditComponent.prototype.resetTaxCodeAndDescription = function () {
        this.handleUpdateProduct({ target: { value: null } }, 'Product.xp.Tax.Code');
        this.handleUpdateProduct({ target: { value: null } }, 'Product.xp.Tax.Description');
    };
    ProductEditComponent.prototype.searchTaxCodes = function (searchTerm) {
        return __awaiter(this, void 0, void 0, function () {
            var taxCodeCategory, avalaraTaxCodes;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (searchTerm === undefined)
                            searchTerm = '';
                        taxCodeCategory = this._superMarketplaceProductEditable.Product.xp.Tax.Category;
                        return [4 /*yield*/, this.middleware.listTaxCodes(taxCodeCategory, searchTerm, 1, 100)];
                    case 1:
                        avalaraTaxCodes = _a.sent();
                        this.taxCodes = avalaraTaxCodes;
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductEditComponent.prototype.handleScrollEnd = function (searchTerm) {
        return __awaiter(this, void 0, void 0, function () {
            var totalPages, nextPageNumber, taxCodeCategory, avalaraTaxCodes;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (searchTerm === undefined)
                            searchTerm = '';
                        totalPages = this.taxCodes.Meta.TotalPages;
                        nextPageNumber = this.taxCodes.Meta.Page + 1;
                        if (!(totalPages > nextPageNumber)) return [3 /*break*/, 2];
                        taxCodeCategory = this._superMarketplaceProductEditable.Product.xp.Tax.Category;
                        return [4 /*yield*/, this.middleware.listTaxCodes(taxCodeCategory, searchTerm, nextPageNumber, 100)];
                    case 1:
                        avalaraTaxCodes = _a.sent();
                        this.taxCodes = {
                            Meta: avalaraTaxCodes.Meta,
                            Items: __spread(this.taxCodes.Items, avalaraTaxCodes.Items),
                        };
                        this.changeDetectorRef.detectChanges();
                        _a.label = 2;
                    case 2: return [2 /*return*/];
                }
            });
        });
    };
    var _a;
    __decorate([
        Input(),
        __metadata("design:type", FormGroup)
    ], ProductEditComponent.prototype, "productForm", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], ProductEditComponent.prototype, "orderCloudProduct", null);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ProductEditComponent.prototype, "filterConfig", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], ProductEditComponent.prototype, "updateResource", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ProductEditComponent.prototype, "addresses", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Boolean)
    ], ProductEditComponent.prototype, "isCreatingNew", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ProductEditComponent.prototype, "dataIsSaving", void 0);
    ProductEditComponent = __decorate([
        Component({
            selector: 'app-product-edit',
            templateUrl: './product-edit.component.html',
            styleUrls: ['./product-edit.component.scss'],
        }),
        __param(11, Inject(applicationConfiguration)),
        __metadata("design:paramtypes", [ChangeDetectorRef,
            Router,
            CurrentUserService,
            OcSupplierAddressService,
            OcProductService,
            OcAdminAddressService, typeof (_a = typeof ProductService !== "undefined" && ProductService) === "function" ? _a : Object, MiddlewareAPIService,
            DomSanitizer,
            NgbModal,
            ToastrService, Object])
    ], ProductEditComponent);
    return ProductEditComponent;
}());
export { ProductEditComponent };
//# sourceMappingURL=product-edit.component.js.map