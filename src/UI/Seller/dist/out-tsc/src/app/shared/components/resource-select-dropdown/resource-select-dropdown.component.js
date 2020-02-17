import { __awaiter, __decorate, __generator, __metadata } from "tslib";
import { Component, Input, ViewChild, ChangeDetectorRef } from '@angular/core';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { faFilter, faHome } from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { Router, ActivatedRoute } from '@angular/router';
import { takeWhile } from 'rxjs/operators';
import { singular } from 'pluralize';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
import { getPsHeight } from '@app-seller/shared/services/dom.helper';
var ResourceSelectDropdown = /** @class */ (function () {
    function ResourceSelectDropdown(router, activatedRoute, changeDetectorRef) {
        this.router = router;
        this.activatedRoute = activatedRoute;
        this.changeDetectorRef = changeDetectorRef;
        this.faFilter = faFilter;
        this.faHome = faHome;
        this.searchTerm = '';
        this.selectedParentResourceName = 'Fetching Data';
        this.alive = true;
        this.resourceSelectDropdownHeight = 450;
        this.parentResourceList = { Meta: {}, Items: [] };
    }
    ResourceSelectDropdown.prototype.ngOnInit = function () {
        this.setParentResourceSubscription();
        this.setParentResourceSelectionSubscription();
    };
    ResourceSelectDropdown.prototype.ngAfterViewChecked = function () {
        // TODO: Magic number ... the 'search' element doesn't exist in the DOM at time of instantiation
        this.resourceSelectDropdownHeight = getPsHeight('additional-item-resource-select-dropdown') - 75;
    };
    ResourceSelectDropdown.prototype.setParentResourceSubscription = function () {
        var _this = this;
        this.parentService.resourceSubject.pipe(takeWhile(function () { return _this.alive; })).subscribe(function (resourceList) {
            _this.parentResourceList = resourceList;
            _this.changeDetectorRef.detectChanges();
        });
    };
    ResourceSelectDropdown.prototype.setParentResourceSelectionSubscription = function () {
        var _this = this;
        this.activatedRoute.params.pipe(takeWhile(function () { return _this.alive; })).subscribe(function (params) { return __awaiter(_this, void 0, void 0, function () {
            var parentIDParamName, resourceID, resource;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!(this.parentService.getParentResourceID() !== REDIRECT_TO_FIRST_PARENT)) return [3 /*break*/, 2];
                        parentIDParamName = singular(this.parentService.primaryResourceLevel) + "ID";
                        resourceID = params[parentIDParamName];
                        if (!(params && resourceID)) return [3 /*break*/, 2];
                        return [4 /*yield*/, this.parentService.findOrGetResourceByID(resourceID)];
                    case 1:
                        resource = _a.sent();
                        this.selectedParentResourceName = resource.Name;
                        _a.label = 2;
                    case 2: return [2 /*return*/];
                }
            });
        }); });
    };
    ResourceSelectDropdown.prototype.selectParentResource = function (resource) {
        this.ocService.selectParentResource(resource);
        // reset the search form when selecting resource
        this.parentService.listResources();
        this.searchTerm = '';
    };
    ResourceSelectDropdown.prototype.searchedResources = function (searchText) {
        this.parentService.listResources(1, searchText);
        this.searchTerm = searchText;
    };
    ResourceSelectDropdown.prototype.handleScrollEnd = function () {
        var totalPages = this.parentResourceList.Meta.TotalPages;
        var nextPageNumber = this.parentResourceList.Meta.Page + 1;
        if (totalPages >= nextPageNumber)
            this.parentService.listResources(nextPageNumber, this.searchTerm);
    };
    ResourceSelectDropdown.prototype.ngOnDestroy = function () {
        this.alive = false;
    };
    __decorate([
        ViewChild('popover', { static: false }),
        __metadata("design:type", NgbPopover)
    ], ResourceSelectDropdown.prototype, "popover", void 0);
    __decorate([
        Input(),
        __metadata("design:type", ResourceCrudService)
    ], ResourceSelectDropdown.prototype, "ocService", void 0);
    __decorate([
        Input(),
        __metadata("design:type", ResourceCrudService)
    ], ResourceSelectDropdown.prototype, "parentService", void 0);
    ResourceSelectDropdown = __decorate([
        Component({
            selector: 'resource-select-dropdown-component',
            templateUrl: './resource-select-dropdown.component.html',
            styleUrls: ['./resource-select-dropdown.component.scss'],
        }),
        __metadata("design:paramtypes", [Router,
            ActivatedRoute,
            ChangeDetectorRef])
    ], ResourceSelectDropdown);
    return ResourceSelectDropdown;
}());
export { ResourceSelectDropdown };
//# sourceMappingURL=resource-select-dropdown.component.js.map