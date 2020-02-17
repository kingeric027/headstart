import { __assign, __awaiter, __generator, __read, __rest, __spread } from "tslib";
import { BehaviorSubject } from 'rxjs';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
import { SUCCESSFUL_WITH_ITEMS, ERROR, GETTING_NEW_ITEMS, FETCHING_SUBSEQUENT_PAGES, REFRESHING_ITEMS, SUCCESSFUL_NO_ITEMS_WITH_FILTERS, SUCCESSFUL_NO_ITEMS_NO_FILTERS, } from './resource-crud.types';
var ResourceCrudService = /** @class */ (function () {
    function ResourceCrudService(router, activatedRoute, ocService, route, primaryResourceLevel, subResourceList, secondaryResourceLevel) {
        var _this = this;
        if (subResourceList === void 0) { subResourceList = []; }
        if (secondaryResourceLevel === void 0) { secondaryResourceLevel = ''; }
        this.router = router;
        this.activatedRoute = activatedRoute;
        this.ocService = ocService;
        this.resourceSubject = new BehaviorSubject({ Meta: {}, Items: [] });
        this.resourceRequestStatus = new BehaviorSubject(GETTING_NEW_ITEMS);
        this.optionsSubject = new BehaviorSubject({});
        this.route = '';
        this.primaryResourceLevel = '';
        // example: for supplier user service the primary is supplier and the secondary is users
        this.secondaryResourceLevel = '';
        this.emptyResource = {};
        this.itemsPerPage = 100;
        this.route = route;
        this.primaryResourceLevel = primaryResourceLevel;
        this.secondaryResourceLevel = secondaryResourceLevel;
        this.subResourceList = subResourceList;
        this.activatedRoute.queryParams.subscribe(function (params) {
            // this prevents service from reading from query params when not on the route related to the service
            if (_this.isOnRelatedRoute()) {
                _this.readFromUrlQueryParams(params);
            }
            else {
                _this.optionsSubject.next({});
            }
        });
        this.optionsSubject.subscribe(function (options) {
            if (_this.getParentResourceID() !== REDIRECT_TO_FIRST_PARENT) {
                _this.listResources();
            }
        });
    }
    ResourceCrudService.prototype.getMyResource = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        console.log('get my resource for this resource not defined');
                        return [4 /*yield*/, Promise.resolve('')];
                    case 1: return [2 /*return*/, _a.sent()];
                }
            });
        });
    };
    ResourceCrudService.prototype.listResources = function (pageNumber, searchText) {
        if (pageNumber === void 0) { pageNumber = 1; }
        if (searchText === void 0) { searchText = ''; }
        return __awaiter(this, void 0, void 0, function () {
            var _a, sortBy, search, filters, OrderDirection, options, resourceResponse;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (!this.shouldListResources()) return [3 /*break*/, 2];
                        _a = this.optionsSubject.value, sortBy = _a.sortBy, search = _a.search, filters = _a.filters, OrderDirection = _a.OrderDirection;
                        options = {
                            page: pageNumber,
                            // allows a list call to pass in a search term that will not appear in the query params
                            search: searchText || search,
                            sortBy: sortBy,
                            pageSize: this.itemsPerPage,
                            filters: filters,
                        };
                        return [4 /*yield*/, this.listWithStatusIndicator(options, OrderDirection)];
                    case 1:
                        resourceResponse = _b.sent();
                        if (pageNumber === 1) {
                            this.setNewResources(resourceResponse);
                        }
                        else {
                            this.addResources(resourceResponse);
                        }
                        _b.label = 2;
                    case 2: return [2 /*return*/];
                }
            });
        });
    };
    ResourceCrudService.prototype.getFetchStatus = function (options) {
        var isSubsequentPage = options.page > 1;
        var areCurrentlyItems = this.resourceSubject.value.Items.length;
        // will not want to show a loading indicator in certain situations so this
        // differentiates between refreshes and new lists
        // when filters are applied REFRESHING_ITEMS will be returned
        if (!areCurrentlyItems && !isSubsequentPage) {
            return GETTING_NEW_ITEMS;
        }
        if (!isSubsequentPage && areCurrentlyItems) {
            return REFRESHING_ITEMS;
        }
        if (isSubsequentPage && areCurrentlyItems) {
            return FETCHING_SUBSEQUENT_PAGES;
        }
        // return isSubsequentPage || !areCurrentlyItems ? GETTING_NEW_ITEMS : REFRESHING_ITEMS;
    };
    ResourceCrudService.prototype.getSucessStatus = function (options, resourceResponse) {
        var areFilters = this.areFiltersOnOptions(options);
        var areItems = !!resourceResponse.Items.length;
        if (areItems)
            return SUCCESSFUL_WITH_ITEMS;
        return areFilters ? SUCCESSFUL_NO_ITEMS_WITH_FILTERS : SUCCESSFUL_NO_ITEMS_NO_FILTERS;
    };
    ResourceCrudService.prototype.shouldListResources = function () {
        if (!this.secondaryResourceLevel) {
            // for primary resources list if on the route
            return this.router.url.startsWith(this.route);
        }
        else {
            // for secondary resources list there is a parent ID
            return !!this.getParentResourceID() && this.router.url.includes(this.secondaryResourceLevel);
        }
    };
    ResourceCrudService.prototype.constructResourceURLs = function (resourceID) {
        if (resourceID === void 0) { resourceID = ''; }
        var newUrlPieces = [];
        newUrlPieces.push(this.route);
        if (this.secondaryResourceLevel) {
            newUrlPieces.push("/" + this.getParentResourceID());
            newUrlPieces.push("/" + this.secondaryResourceLevel);
        }
        if (resourceID) {
            newUrlPieces.push("/" + resourceID);
        }
        return newUrlPieces;
    };
    ResourceCrudService.prototype.selectParentResource = function (resource) {
        var _this = this;
        var newUrl = this.updateUrlForUpdatedParent(resource);
        this.router.navigateByUrl(newUrl);
        // this settimeout ensures that the new parent resource ID is in the url before the resources are listed
        // find a better way to update the resources on the parent resource ID change
        setTimeout(function () {
            _this.listResources();
        });
    };
    ResourceCrudService.prototype.updateUrlForUpdatedParent = function (resource) {
        var queryParams = this.router.url.split('?')[1];
        var newUrl = this.primaryResourceLevel + "/" + resource.ID + "/" + this.secondaryResourceLevel;
        if (queryParams) {
            newUrl += "?" + queryParams;
        }
        return newUrl;
    };
    ResourceCrudService.prototype.constructNewRouteInformation = function (resourceID) {
        if (resourceID === void 0) { resourceID = ''; }
        var newUrl = '';
        var queryParams = this.activatedRoute.snapshot.queryParams;
        if (this.secondaryResourceLevel) {
            newUrl += this.route + "/" + this.getParentResourceID() + "/" + this.secondaryResourceLevel;
        }
        else {
            newUrl += "" + this.route;
        }
        if (resourceID) {
            newUrl += "/" + resourceID;
        }
        return [newUrl, queryParams];
    };
    ResourceCrudService.prototype.getParentResourceID = function () {
        var urlPieces = this.router.url.split('/');
        var indexOfParent = urlPieces.indexOf("" + this.primaryResourceLevel);
        return urlPieces[indexOfParent + 1];
    };
    ResourceCrudService.prototype.getResourceById = function (resourceID) {
        var _a;
        var orderDirection = this.optionsSubject.value.OrderDirection;
        return (_a = this.ocService).Get.apply(_a, __spread(this.createListArgs([resourceID], orderDirection))).toPromise();
    };
    ResourceCrudService.prototype.createListArgs = function (options, orderDirection) {
        if (orderDirection === void 0) { orderDirection = ''; }
        /* ordercloud services follow a patter where the paramters to a function (Save, Create, List)
          are the nearly the same for all resource. However, sub resources (supplier users, buyer payment methods, etc...)
          have the parent resource ID as the first paramter before the expected argument
        */
        if (this.primaryResourceLevel === 'orders') {
            // placeholder conditional for getting the supplier order list page running
            // will need to integrate this with the filter on the order list page as a seller
            // user and potentially refactor later
            return __spread([orderDirection || 'Incoming'], options);
        }
        if (this.secondaryResourceLevel) {
            var parentResourceID = this.getParentResourceID();
            return __spread([parentResourceID], options);
        }
        else {
            return __spread(options);
        }
    };
    ResourceCrudService.prototype.findOrGetResourceByID = function (resourceID) {
        return __awaiter(this, void 0, void 0, function () {
            var resourceInList;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        resourceInList = this.resourceSubject.value.Items.find(function (i) { return i.ID === resourceID; });
                        if (!resourceInList) return [3 /*break*/, 1];
                        return [2 /*return*/, resourceInList];
                    case 1:
                        if (!(resourceID !== REDIRECT_TO_FIRST_PARENT)) return [3 /*break*/, 3];
                        return [4 /*yield*/, this.getResourceById(resourceID)];
                    case 2: return [2 /*return*/, _a.sent()];
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    ResourceCrudService.prototype.updateResource = function (resource) {
        return __awaiter(this, void 0, void 0, function () {
            var newResource, resourceIndex;
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, (_a = this.ocService).Save.apply(_a, __spread(this.createListArgs([resource.ID, resource]))).toPromise()];
                    case 1:
                        newResource = _b.sent();
                        resourceIndex = this.resourceSubject.value.Items.findIndex(function (i) { return i.ID === newResource.ID; });
                        this.resourceSubject.value.Items[resourceIndex] = newResource;
                        this.resourceSubject.next(this.resourceSubject.value);
                        return [2 /*return*/, newResource];
                }
            });
        });
    };
    ResourceCrudService.prototype.deleteResource = function (resourceID) {
        return __awaiter(this, void 0, void 0, function () {
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, (_a = this.ocService).Delete.apply(_a, __spread(this.createListArgs([resourceID]))).toPromise()];
                    case 1:
                        _b.sent();
                        this.resourceSubject.value.Items = this.resourceSubject.value.Items.filter(function (i) { return i.ID !== resourceID; });
                        this.resourceSubject.next(this.resourceSubject.value);
                        return [2 /*return*/];
                }
            });
        });
    };
    ResourceCrudService.prototype.createNewResource = function (resource) {
        return __awaiter(this, void 0, void 0, function () {
            var newResource;
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, (_a = this.ocService).Create.apply(_a, __spread(this.createListArgs([resource]))).toPromise()];
                    case 1:
                        newResource = _b.sent();
                        this.resourceSubject.value.Items = __spread(this.resourceSubject.value.Items, [newResource]);
                        this.resourceSubject.next(this.resourceSubject.value);
                        return [2 /*return*/, newResource];
                }
            });
        });
    };
    ResourceCrudService.prototype.setNewResources = function (resourceResponse) {
        this.resourceSubject.next(resourceResponse);
    };
    ResourceCrudService.prototype.addResources = function (resourceResponse) {
        this.resourceSubject.next({
            Meta: resourceResponse.Meta,
            Items: __spread(this.resourceSubject.value.Items, resourceResponse.Items),
        });
    };
    ResourceCrudService.prototype.getNextPage = function () {
        if (this.resourceSubject.value.Meta && this.resourceSubject.value.Meta.Page) {
            this.listResources(this.resourceSubject.value.Meta.Page + 1);
        }
    };
    ResourceCrudService.prototype.patchFilterState = function (patch) {
        var activeOptions = __assign(__assign({}, this.optionsSubject.value), patch);
        var queryParams = this.mapToUrlQueryParams(activeOptions);
        this.router.navigate([], { queryParams: queryParams }); // update url, which will call readFromUrlQueryParams()
    };
    ResourceCrudService.prototype.toPage = function (pageNumber) {
        this.patchFilterState({ page: pageNumber || undefined });
    };
    ResourceCrudService.prototype.sortBy = function (field) {
        this.patchFilterState({ sortBy: field || undefined });
    };
    ResourceCrudService.prototype.searchBy = function (searchTerm) {
        this.patchFilterState({ search: searchTerm || undefined });
    };
    ResourceCrudService.prototype.addFilters = function (newFilters) {
        var newFilterDictionary = __assign(__assign({}, this.optionsSubject.value.filters), newFilters);
        this.patchFilterState({ filters: newFilterDictionary });
    };
    ResourceCrudService.prototype.removeFilters = function (filtersToRemove) {
        var newFilterDictionary = __assign({}, this.optionsSubject.value.filters);
        filtersToRemove.forEach(function (filter) {
            if (newFilterDictionary[filter]) {
                delete newFilterDictionary[filter];
            }
        });
        this.patchFilterState({ filters: newFilterDictionary });
    };
    ResourceCrudService.prototype.clearSort = function () {
        this.sortBy(undefined);
    };
    ResourceCrudService.prototype.clearSearch = function () {
        this.searchBy(undefined);
    };
    ResourceCrudService.prototype.clearAllFilters = function () {
        this.patchFilterState({ filters: {} });
    };
    ResourceCrudService.prototype.clearResources = function () {
        this.resourceSubject.next({ Meta: {}, Items: [] });
    };
    ResourceCrudService.prototype.getRouteFromResourceName = function (resourceName) {
        return "/" + resourceName;
    };
    ResourceCrudService.prototype.hasFilters = function () {
        var filters = this.optionsSubject.value;
        return Object.entries(filters).some(function (_a) {
            var _b = __read(_a, 2), key = _b[0], value = _b[1];
            return !!value;
        });
    };
    ResourceCrudService.prototype.areFiltersOnOptions = function (options) {
        return (!!options.search ||
            (options.filters &&
                Object.entries(options.filters).some(function (_a) {
                    var _b = __read(_a, 2), key = _b[0], value = _b[1];
                    return !!value;
                })));
    };
    // Used to update the URL
    ResourceCrudService.prototype.mapToUrlQueryParams = function (options) {
        var sortBy = options.sortBy, search = options.search, filters = options.filters, OrderDirection = options.OrderDirection;
        return __assign(__assign({ sortBy: sortBy, search: search }, filters), { OrderDirection: OrderDirection });
    };
    // Handle URL updates
    ResourceCrudService.prototype.readFromUrlQueryParams = function (params) {
        var sortBy = params.sortBy, search = params.search, OrderDirection = params.OrderDirection, filters = __rest(params, ["sortBy", "search", "OrderDirection"]);
        this.optionsSubject.next({ sortBy: sortBy, search: search, filters: filters, OrderDirection: OrderDirection });
    };
    ResourceCrudService.prototype.listWithStatusIndicator = function (options, orderDirection) {
        if (orderDirection === void 0) { orderDirection = ''; }
        return __awaiter(this, void 0, void 0, function () {
            var resourceResponse, error_1;
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        _b.trys.push([0, 2, , 3]);
                        this.resourceRequestStatus.next(this.getFetchStatus(options));
                        return [4 /*yield*/, (_a = this.ocService).List.apply(_a, __spread(this.createListArgs([options], orderDirection))).toPromise()];
                    case 1:
                        resourceResponse = _b.sent();
                        this.resourceRequestStatus.next(this.getSucessStatus(options, resourceResponse));
                        return [2 /*return*/, resourceResponse];
                    case 2:
                        error_1 = _b.sent();
                        this.resourceRequestStatus.next(ERROR);
                        throw error_1;
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    ResourceCrudService.prototype.isOnRelatedRoute = function () {
        var _this = this;
        var isOnSubResource = this.subResourceList &&
            this.subResourceList.some(function (subResource) {
                return _this.router.url.includes("/" + subResource);
            });
        var isOnBaseRoute = this.router.url.includes(this.route);
        var isOnRelatedSubResource = this.router.url.includes("/" + this.secondaryResourceLevel);
        if (!isOnBaseRoute) {
            return false;
        }
        else if (isOnSubResource && this.secondaryResourceLevel && isOnRelatedSubResource) {
            return true;
        }
        else if (!isOnSubResource && !this.secondaryResourceLevel) {
            return true;
        }
        else {
            return false;
        }
    };
    return ResourceCrudService;
}());
export { ResourceCrudService };
//# sourceMappingURL=resource-crud.service.js.map