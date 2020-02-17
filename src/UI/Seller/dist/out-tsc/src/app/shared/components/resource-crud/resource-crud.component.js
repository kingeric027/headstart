import { __awaiter, __generator, __read } from "tslib";
import { takeWhile } from 'rxjs/operators';
import { singular } from 'pluralize';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
var ResourceCrudComponent = /** @class */ (function () {
    function ResourceCrudComponent(changeDetectorRef, ocService, router, activatedRoute, ngZone, createForm) {
        this.changeDetectorRef = changeDetectorRef;
        this.activatedRoute = activatedRoute;
        this.ngZone = ngZone;
        this.alive = true;
        this.resourceList = { Meta: {}, Items: [] };
        // empty string if no resource is selected
        this.selectedResourceID = '';
        this.updatedResource = {};
        this.resourceInSelection = {};
        this.isMyResource = false;
        this.filterConfig = {};
        this.dataIsSaving = false;
        this.ocService = ocService;
        this.router = router;
        this.createForm = createForm;
    }
    ResourceCrudComponent.prototype.navigate = function (url, options) {
        var _this = this;
        /*
        * Had a bug where clicking on a resource on the second page of resources was triggering an error
        * navigation trigger outside of Angular zone. Might be caused by inheritance or using
        * changeDetector.detectChange, but couldn't resolve any other way
        * Please remove the need for this if you can
        * https://github.com/angular/angular/issues/25837
        */
        if (Object.keys(options)) {
            this.ngZone.run(function () { return _this.router.navigate([url], options); }).then();
        }
        else {
            this.ngZone.run(function () { return _this.router.navigate([url]); }).then();
        }
    };
    ResourceCrudComponent.prototype.ngOnInit = function () {
        this.determineViewingContext();
        this.subscribeToResources();
        this.subscribeToResourceSelection();
        this.setForm(this.updatedResource);
    };
    ResourceCrudComponent.prototype.subscribeToResources = function () {
        var _this = this;
        this.ocService.resourceSubject.pipe(takeWhile(function () { return _this.alive; })).subscribe(function (resourceList) {
            _this.resourceList = resourceList;
            _this.changeDetectorRef.detectChanges();
        });
    };
    ResourceCrudComponent.prototype.determineViewingContext = function () {
        return __awaiter(this, void 0, void 0, function () {
            var supplier;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.isMyResource = this.router.url.startsWith('/my-');
                        if (!this.isMyResource) return [3 /*break*/, 2];
                        return [4 /*yield*/, this.ocService.getMyResource()];
                    case 1:
                        supplier = _a.sent();
                        this.setResourceSelectionFromResource(supplier);
                        _a.label = 2;
                    case 2: return [2 /*return*/];
                }
            });
        });
    };
    ResourceCrudComponent.prototype.subscribeToResourceSelection = function () {
        var _this = this;
        this.activatedRoute.params.subscribe(function (params) {
            if (_this.ocService.getParentResourceID() !== REDIRECT_TO_FIRST_PARENT) {
                _this.setIsCreatingNew();
                var resourceIDSelected = params[singular(_this.ocService.secondaryResourceLevel || _this.ocService.primaryResourceLevel) + "ID"];
                if (resourceIDSelected) {
                    _this.setResourceSelectionFromID(resourceIDSelected);
                }
                if (_this.isCreatingNew) {
                    _this.setResoureObjectsForCreatingNew();
                }
            }
        });
    };
    ResourceCrudComponent.prototype.setForm = function (resource) {
        if (this.createForm) {
            this.resourceForm = this.createForm(resource);
            this.changeDetectorRef.detectChanges();
        }
    };
    ResourceCrudComponent.prototype.resetForm = function (resource) {
        if (this.createForm) {
            this.resourceForm.reset(this.createForm(resource));
            this.changeDetectorRef.detectChanges();
        }
    };
    ResourceCrudComponent.prototype.handleScrollEnd = function () {
        if (this.resourceList.Meta.TotalPages > this.resourceList.Meta.Page) {
            this.ocService.getNextPage();
        }
    };
    ResourceCrudComponent.prototype.searchResources = function (searchStr) {
        this.ocService.searchBy(searchStr);
    };
    ResourceCrudComponent.prototype.setResourceSelectionFromID = function (resourceID) {
        return __awaiter(this, void 0, void 0, function () {
            var resource;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.selectedResourceID = resourceID || '';
                        return [4 /*yield*/, this.ocService.findOrGetResourceByID(resourceID)];
                    case 1:
                        resource = _a.sent();
                        this.resourceInSelection = this.copyResource(resource);
                        this.setUpdatedResourceAndResourceForm(resource);
                        return [2 /*return*/];
                }
            });
        });
    };
    ResourceCrudComponent.prototype.setResourceSelectionFromResource = function (resource) {
        this.selectedResourceID = (resource && resource.ID) || '';
        this.resourceInSelection = this.copyResource(resource);
        this.setUpdatedResourceAndResourceForm(resource);
    };
    ResourceCrudComponent.prototype.setResoureObjectsForCreatingNew = function () {
        this.resourceInSelection = this.ocService.emptyResource;
        this.setUpdatedResourceAndResourceForm(this.ocService.emptyResource);
    };
    ResourceCrudComponent.prototype.selectResource = function (resource) {
        var _a = __read(this.ocService.constructNewRouteInformation(resource.ID || ''), 2), newURL = _a[0], queryParams = _a[1];
        this.navigate(newURL, { queryParams: queryParams });
    };
    ResourceCrudComponent.prototype.updateResource = function (resourceUpdate) {
        // copying a resetting this.updated resource ensures that the copy and base object
        // reference is broken
        // not the prettiest function, feel free to improve
        var piecesOfField = resourceUpdate.field.split('.');
        var depthOfField = piecesOfField.length;
        var updatedResourceCopy = this.copyResource(this.updatedResource);
        switch (depthOfField) {
            case 4:
                updatedResourceCopy[piecesOfField[0]][piecesOfField[1]][piecesOfField[2]][piecesOfField[3]] =
                    resourceUpdate.value;
                break;
            case 3:
                updatedResourceCopy[piecesOfField[0]][piecesOfField[1]][piecesOfField[2]] = resourceUpdate.value;
                break;
            case 2:
                updatedResourceCopy[piecesOfField[0]][piecesOfField[1]] = resourceUpdate.value;
                break;
            default:
                updatedResourceCopy[piecesOfField[0]] = resourceUpdate.value;
                break;
        }
        this.updatedResource = updatedResourceCopy;
        this.changeDetectorRef.detectChanges();
    };
    ResourceCrudComponent.prototype.handleUpdateResource = function (event, field) {
        var resourceUpdate = {
            field: field,
            value: field === 'Active' ? event.target.checked : event.target.value,
        };
        this.updateResource(resourceUpdate);
    };
    ResourceCrudComponent.prototype.copyResource = function (resource) {
        return JSON.parse(JSON.stringify(resource));
    };
    ResourceCrudComponent.prototype.saveUpdates = function () {
        if (this.isCreatingNew) {
            this.createNewResource();
        }
        else {
            this.updateExitingResource();
        }
    };
    ResourceCrudComponent.prototype.deleteResource = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.ocService.deleteResource(this.selectedResourceID)];
                    case 1:
                        _a.sent();
                        this.selectResource({});
                        return [2 /*return*/];
                }
            });
        });
    };
    ResourceCrudComponent.prototype.discardChanges = function () {
        this.setUpdatedResourceAndResourceForm(this.resourceInSelection);
    };
    ResourceCrudComponent.prototype.updateExitingResource = function () {
        return __awaiter(this, void 0, void 0, function () {
            var updatedResource, ex_1;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        this.dataIsSaving = true;
                        return [4 /*yield*/, this.ocService.updateResource(this.updatedResource)];
                    case 1:
                        updatedResource = _a.sent();
                        this.resourceInSelection = this.copyResource(updatedResource);
                        this.setUpdatedResourceAndResourceForm(updatedResource);
                        this.dataIsSaving = false;
                        return [3 /*break*/, 3];
                    case 2:
                        ex_1 = _a.sent();
                        this.dataIsSaving = false;
                        throw ex_1;
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    ResourceCrudComponent.prototype.setUpdatedResourceAndResourceForm = function (updatedResource) {
        this.updatedResource = this.copyResource(updatedResource);
        this.setForm(this.copyResource(updatedResource));
        this.changeDetectorRef.detectChanges();
    };
    ResourceCrudComponent.prototype.createNewResource = function () {
        return __awaiter(this, void 0, void 0, function () {
            var newResource, ex_2;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        this.dataIsSaving = true;
                        return [4 /*yield*/, this.ocService.createNewResource(this.updatedResource)];
                    case 1:
                        newResource = _a.sent();
                        this.selectResource(newResource);
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
    ResourceCrudComponent.prototype.ngOnDestroy = function () {
        this.alive = false;
    };
    ResourceCrudComponent.prototype.setIsCreatingNew = function () {
        var routeUrl = this.router.routerState.snapshot.url;
        var endUrl = routeUrl.slice(routeUrl.length - 4, routeUrl.length);
        this.isCreatingNew = endUrl === '/new';
    };
    return ResourceCrudComponent;
}());
export { ResourceCrudComponent };
//# sourceMappingURL=resource-crud.component.js.map