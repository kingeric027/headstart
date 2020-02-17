import { __decorate, __metadata, __param } from "tslib";
import { Component, Inject, Input } from '@angular/core';
import { OcCategoryService } from '@ordercloud/angular-sdk';
import { faSitemap, faBoxOpen } from '@fortawesome/free-solid-svg-icons';
import { ActivatedRoute, Router } from '@angular/router';
import { flatMap } from 'rxjs/operators';
import { applicationConfiguration } from '@app-seller/config/app.config';
var CategoryDetailsComponent = /** @class */ (function () {
    function CategoryDetailsComponent(activatedRoute, ocCategoryService, router, appConfig) {
        this.activatedRoute = activatedRoute;
        this.ocCategoryService = ocCategoryService;
        this.router = router;
        this.appConfig = appConfig;
        this.faSitemap = faSitemap;
        this.faBoxOpen = faBoxOpen;
    }
    CategoryDetailsComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.getCategoryData().subscribe(function (x) { return (_this.category = x); });
    };
    CategoryDetailsComponent.prototype.getCategoryData = function () {
        var _this = this;
        return this.activatedRoute.params.pipe(flatMap(function (params) {
            if (params.categoryID) {
                _this.categoryID = params.categoryID;
                return _this.ocCategoryService.Get(_this.catalogID, params.categoryID);
            }
        }));
    };
    CategoryDetailsComponent.prototype.updateCategory = function (category) {
        var _this = this;
        this.ocCategoryService.Patch(this.catalogID, this.categoryID, category).subscribe(function (x) {
            _this.category = x;
            if (_this.category.ID !== _this.categoryID) {
                _this.router.navigateByUrl("/categories/" + _this.category.ID);
            }
        });
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], CategoryDetailsComponent.prototype, "catalogID", void 0);
    CategoryDetailsComponent = __decorate([
        Component({
            selector: 'category-details',
            templateUrl: './category-details.component.html',
            styleUrls: ['./category-details.component.scss'],
        }),
        __param(3, Inject(applicationConfiguration)),
        __metadata("design:paramtypes", [ActivatedRoute,
            OcCategoryService,
            Router, Object])
    ], CategoryDetailsComponent);
    return CategoryDetailsComponent;
}());
export { CategoryDetailsComponent };
//# sourceMappingURL=category-details.component.js.map