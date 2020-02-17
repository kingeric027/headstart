/**
 * All the OC API "List" endpoints share a common pagination, searching, and sorting functionality.
 *
 * Use this as a super class for table or list components that need those features.
 */
var BaseBrowse = /** @class */ (function () {
    function BaseBrowse() {
        this.requestOptions = { search: undefined, page: undefined, sortBy: undefined };
    }
    BaseBrowse.prototype.pageChanged = function (page) {
        Object.assign(this.requestOptions, { page: page });
        this.loadData();
    };
    BaseBrowse.prototype.searchChanged = function (searchStr) {
        Object.assign(this.requestOptions, { search: searchStr, page: undefined });
        this.loadData();
    };
    BaseBrowse.prototype.sortChanged = function (sortStr) {
        Object.assign(this.requestOptions, { sortBy: sortStr, page: undefined });
        this.loadData();
    };
    return BaseBrowse;
}());
export { BaseBrowse };
//# sourceMappingURL=base-browse.class.js.map