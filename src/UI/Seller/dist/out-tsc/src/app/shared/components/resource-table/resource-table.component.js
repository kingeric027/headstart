import { __assign, __awaiter, __decorate, __generator, __metadata, __read } from "tslib";
import { ChangeDetectorRef, Component, EventEmitter, Input, NgZone, Output, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
import { getPsHeight, getScreenSizeBreakPoint } from '@app-seller/shared/services/dom.helper';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { faCalendar, faChevronLeft, faFilter, faHome, faTimes } from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { singular } from 'pluralize';
import { filter, takeWhile } from 'rxjs/operators';
var ResourceTableComponent = /** @class */ (function () {
    function ResourceTableComponent(router, activatedRoute, changeDetectorRef, ngZone) {
        this.router = router;
        this.activatedRoute = activatedRoute;
        this.changeDetectorRef = changeDetectorRef;
        this.faFilter = faFilter;
        this.faTimes = faTimes;
        this.faHome = faHome;
        this.faChevronLeft = faChevronLeft;
        this.faCalendar = faCalendar;
        this.searchTerm = '';
        this.selectedParentResourceName = 'Fetching Data';
        this.selectedParentResourceID = '';
        this.breadCrumbs = [];
        this.isCreatingNew = false;
        this.isMyResource = false;
        this.alive = true;
        this.myResourceHeight = 450;
        this.tableHeight = 450;
        this.editResourceHeight = 450;
        this.activeFilterCount = 0;
        this.resourceList = { Meta: {}, Items: [] };
        this.searched = new EventEmitter();
        this.hitScrollEnd = new EventEmitter();
        this.changesSaved = new EventEmitter();
        this.resourceDelete = new EventEmitter();
        this.changesDiscarded = new EventEmitter();
        this.resourceSelected = new EventEmitter();
        this.shouldShowCreateNew = true;
        this.shouldShowResourceActions = true;
        this.dataIsSaving = false;
    }
    Object.defineProperty(ResourceTableComponent.prototype, "ocService", {
        set: function (service) {
            this._ocService = service;
            this._currentResourceNamePlural = service.secondaryResourceLevel || service.primaryResourceLevel;
            this._currentResourceNameSingular = singular(this._currentResourceNamePlural);
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ResourceTableComponent.prototype, "updatedResource", {
        set: function (value) {
            this._updatedResource = value;
            this.checkForChanges();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ResourceTableComponent.prototype, "resourceInSelection", {
        set: function (value) {
            this._resourceInSelection = value;
            this.checkForChanges();
        },
        enumerable: true,
        configurable: true
    });
    ResourceTableComponent.prototype.ngOnInit = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                this.determineViewingContext();
                this.initializeSubscriptions();
                this.setFilterForm();
                this.subscribeToOptions();
                this.screenSize = getScreenSizeBreakPoint();
                return [2 /*return*/];
            });
        });
    };
    ResourceTableComponent.prototype.ngAfterViewChecked = function () {
        this.setPsHeights();
        this.changeDetectorRef.detectChanges();
    };
    ResourceTableComponent.prototype.subscribeToOptions = function () {
        var _this = this;
        this._ocService.optionsSubject.pipe(takeWhile(function () { return _this.alive; })).subscribe(function (options) {
            _this.resourceOptions = options;
            _this.searchTerm = (options && options.search) || '';
            _this.activeFilterCount = options.filters ? Object.keys(options.filters).length : 0;
            _this.setFilterForm();
            _this.changeDetectorRef.detectChanges();
        });
    };
    ResourceTableComponent.prototype.applyFilters = function () {
        if (typeof this.filterForm.value['from'] === 'object') {
            var fromDate = this.filterForm.value['from'];
            this.fromDate = this.transformDateForUser(fromDate);
            this.filterForm.value['from'] = this.transformDateForFilter(fromDate);
        }
        if (typeof this.filterForm.value['to'] === 'object') {
            var toDate = this.filterForm.value['to'];
            this.toDate = this.transformDateForUser(toDate);
            this.filterForm.value['to'] = this.transformDateForFilter(toDate);
        }
        this._ocService.addFilters(this.removeFieldsWithNoValue(this.filterForm.value));
    };
    // date format for NgbDatepicker is different than date format used for filters
    ResourceTableComponent.prototype.transformDateForUser = function (date) {
        var month = date.month.toString().length === 1 ? '0' + date.month : date.month;
        var day = date.day.toString().length === 1 ? '0' + date.day : date.day;
        return date.year + '-' + month + '-' + day;
    };
    ResourceTableComponent.prototype.transformDateForFilter = function (date) {
        return date.month + '-' + date.day + '-' + date.year;
    };
    ResourceTableComponent.prototype.removeFieldsWithNoValue = function (formValues) {
        var values = __assign({}, formValues);
        Object.entries(values).forEach(function (_a) {
            var _b = __read(_a, 2), key = _b[0], value = _b[1];
            if (!value) {
                delete values[key];
            }
        });
        return values;
    };
    ResourceTableComponent.prototype.setPsHeights = function () {
        this.myResourceHeight = getPsHeight('');
        this.tableHeight = getPsHeight('additional-item-table');
        this.editResourceHeight = getPsHeight('additional-item-edit-resource');
    };
    ResourceTableComponent.prototype.determineViewingContext = function () {
        this.isMyResource = this.router.url.startsWith('/my-');
    };
    ResourceTableComponent.prototype.initializeSubscriptions = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.redirectToFirstParentIfNeeded()];
                    case 1:
                        _a.sent();
                        this.setUrlSubscription();
                        this.setParentResourceSelectionSubscription();
                        this.setListRequestStatusSubscription();
                        this._ocService.listResources();
                        return [2 /*return*/];
                }
            });
        });
    };
    ResourceTableComponent.prototype.redirectToFirstParentIfNeeded = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!this.parentResourceService) return [3 /*break*/, 2];
                        if (!(this.parentResourceService.getParentResourceID() === REDIRECT_TO_FIRST_PARENT)) return [3 /*break*/, 2];
                        return [4 /*yield*/, this.parentResourceService.listResources()];
                    case 1:
                        _a.sent();
                        this._ocService.selectParentResource(this.parentResourceService.resourceSubject.value.Items[0]);
                        _a.label = 2;
                    case 2: return [2 /*return*/];
                }
            });
        });
    };
    ResourceTableComponent.prototype.setUrlSubscription = function () {
        var _this = this;
        this.router.events
            .pipe(takeWhile(function () { return _this.alive; }))
            // only need to set the breadcrumbs on nav end events
            .pipe(filter(function (event) { return event instanceof NavigationEnd; }))
            .subscribe(function () {
            _this.setBreadCrumbs();
        });
        this.activatedRoute.params.pipe(takeWhile(function () { return _this.alive; })).subscribe(function () {
            _this.setBreadCrumbs();
            _this.checkIfCreatingNew();
        });
    };
    ResourceTableComponent.prototype.setParentResourceSelectionSubscription = function () {
        var _this = this;
        this.activatedRoute.params
            .pipe(takeWhile(function () { return _this.parentResourceService && _this.alive; }))
            .subscribe(function (params) { return __awaiter(_this, void 0, void 0, function () {
            var parentIDParamName, parentResourceID, parentResource;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.redirectToFirstParentIfNeeded()];
                    case 1:
                        _a.sent();
                        parentIDParamName = singular(this._ocService.primaryResourceLevel) + "ID";
                        parentResourceID = params[parentIDParamName];
                        this.selectedParentResourceID = parentResourceID;
                        if (!(params && parentResourceID)) return [3 /*break*/, 3];
                        return [4 /*yield*/, this.parentResourceService.findOrGetResourceByID(parentResourceID)];
                    case 2:
                        parentResource = _a.sent();
                        if (parentResource)
                            this.selectedParentResourceName = parentResource.Name;
                        _a.label = 3;
                    case 3: return [2 /*return*/];
                }
            });
        }); });
    };
    ResourceTableComponent.prototype.setListRequestStatusSubscription = function () {
        var _this = this;
        this._ocService.resourceRequestStatus.pipe(takeWhile(function () { return _this.alive; })).subscribe(function (requestStatus) {
            _this.requestStatus = requestStatus;
            _this.changeDetectorRef.detectChanges();
        });
    };
    ResourceTableComponent.prototype.checkIfCreatingNew = function () {
        var routeUrl = this.router.routerState.snapshot.url;
        var endUrl = routeUrl.slice(routeUrl.length - 4, routeUrl.length);
        this.isCreatingNew = endUrl === '/new';
    };
    ResourceTableComponent.prototype.setBreadCrumbs = function () {
        // basically we are just taking off the portion of the url after the selected route piece
        // in the future breadcrumb logic might need to be more complicated than this
        var urlPieces = this.router.url
            .split('/')
            .filter(function (p) { return p; })
            .map(function (p) {
            if (p.includes('?')) {
                return p.slice(0, p.indexOf('?'));
            }
            else {
                return p;
            }
        });
        this.breadCrumbs = urlPieces.map(function (piece, index) {
            var route = "/" + urlPieces.slice(0, index + 1).join('/');
            return {
                displayText: piece,
                route: route,
            };
        });
        this.changeDetectorRef.detectChanges();
    };
    ResourceTableComponent.prototype.setFilterForm = function () {
        var _this = this;
        var formGroup = {};
        if (this.filterConfig && this.filterConfig.Filters) {
            this.filterConfig.Filters.forEach(function (filter) {
                var value = _this.getSelectedFilterValue(filter.Path);
                formGroup[filter.Path] = new FormControl(value);
            });
            this.filterForm = new FormGroup(formGroup);
        }
    };
    ResourceTableComponent.prototype.getSelectedFilterValue = function (pathOfFilter) {
        return (this.resourceOptions && this.resourceOptions.filters && this.resourceOptions.filters[pathOfFilter]) || '';
    };
    ResourceTableComponent.prototype.searchedResources = function (event) {
        this.searched.emit(event);
    };
    ResourceTableComponent.prototype.handleScrollEnd = function () {
        this.hitScrollEnd.emit(null);
    };
    ResourceTableComponent.prototype.handleSave = function () {
        this.changesSaved.emit(null);
    };
    ResourceTableComponent.prototype.handleDelete = function () {
        this.resourceDelete.emit(null);
    };
    ResourceTableComponent.prototype.handleDiscardChanges = function () {
        this.changesDiscarded.emit(null);
    };
    ResourceTableComponent.prototype.handleSelectResource = function (resource) {
        var _a = __read(this._ocService.constructNewRouteInformation(resource.ID || ''), 2), newURL = _a[0], queryParams = _a[1];
        this.router.navigate([newURL], { queryParams: queryParams });
        this.resourceSelected.emit(resource);
    };
    ResourceTableComponent.prototype.openPopover = function () {
        this.popover.open();
    };
    ResourceTableComponent.prototype.closePopover = function () {
        this.popover.close();
    };
    ResourceTableComponent.prototype.handleApplyFilters = function () {
        this.closePopover();
        this.applyFilters();
    };
    ResourceTableComponent.prototype.clearAllFilters = function () {
        this._ocService.clearAllFilters();
        this.toDate = '';
        this.fromDate = '';
    };
    ResourceTableComponent.prototype.checkForChanges = function () {
        this.areChanges = JSON.stringify(this._updatedResource) !== JSON.stringify(this._resourceInSelection);
    };
    ResourceTableComponent.prototype.ngOnDestroy = function () {
        this.alive = false;
    };
    __decorate([
        ViewChild('popover', { static: false }),
        __metadata("design:type", NgbPopover)
    ], ResourceTableComponent.prototype, "popover", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ResourceTableComponent.prototype, "resourceList", void 0);
    __decorate([
        Input(),
        __metadata("design:type", ResourceCrudService),
        __metadata("design:paramtypes", [ResourceCrudService])
    ], ResourceTableComponent.prototype, "ocService", null);
    __decorate([
        Input(),
        __metadata("design:type", ResourceCrudService)
    ], ResourceTableComponent.prototype, "parentResourceService", void 0);
    __decorate([
        Output(),
        __metadata("design:type", EventEmitter)
    ], ResourceTableComponent.prototype, "searched", void 0);
    __decorate([
        Output(),
        __metadata("design:type", EventEmitter)
    ], ResourceTableComponent.prototype, "hitScrollEnd", void 0);
    __decorate([
        Output(),
        __metadata("design:type", EventEmitter)
    ], ResourceTableComponent.prototype, "changesSaved", void 0);
    __decorate([
        Output(),
        __metadata("design:type", EventEmitter)
    ], ResourceTableComponent.prototype, "resourceDelete", void 0);
    __decorate([
        Output(),
        __metadata("design:type", EventEmitter)
    ], ResourceTableComponent.prototype, "changesDiscarded", void 0);
    __decorate([
        Output(),
        __metadata("design:type", EventEmitter)
    ], ResourceTableComponent.prototype, "resourceSelected", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], ResourceTableComponent.prototype, "updatedResource", null);
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], ResourceTableComponent.prototype, "resourceInSelection", null);
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], ResourceTableComponent.prototype, "selectedResourceID", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ResourceTableComponent.prototype, "filterConfig", void 0);
    __decorate([
        Input(),
        __metadata("design:type", FormGroup)
    ], ResourceTableComponent.prototype, "resourceForm", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ResourceTableComponent.prototype, "shouldShowCreateNew", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ResourceTableComponent.prototype, "shouldShowResourceActions", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ResourceTableComponent.prototype, "dataIsSaving", void 0);
    ResourceTableComponent = __decorate([
        Component({
            selector: 'resource-table-component',
            templateUrl: './resource-table.component.html',
            styleUrls: ['./resource-table.component.scss'],
            host: {
                '(window:resize)': 'ngAfterViewChecked()',
            },
        }),
        __metadata("design:paramtypes", [Router,
            ActivatedRoute,
            ChangeDetectorRef,
            NgZone])
    ], ResourceTableComponent);
    return ResourceTableComponent;
}());
export { ResourceTableComponent };
//# sourceMappingURL=resource-table.component.js.map