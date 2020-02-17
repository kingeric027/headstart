import { __extends } from "tslib";
import { BaseBrowse } from '@app-seller/shared/models/base-browse.class';
var TestBaseBrowse = /** @class */ (function (_super) {
    __extends(TestBaseBrowse, _super);
    function TestBaseBrowse() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    TestBaseBrowse.prototype.loadData = function () { };
    return TestBaseBrowse;
}(BaseBrowse));
var component = new TestBaseBrowse();
describe('BaseBrowse', function () {
    beforeEach(function () {
        spyOn(component, 'loadData');
    });
    it('should search, reseting page', function () {
        component.requestOptions = { page: 2, search: 'hose', sortBy: 'ID' };
        component.searchChanged('newSearch');
        expect(component.requestOptions).toEqual({
            page: undefined,
            search: 'newSearch',
            sortBy: 'ID',
        });
    });
    it('should change page, keeping search + sort', function () {
        component.requestOptions = { page: 2, search: 'hose', sortBy: 'ID' };
        component.pageChanged(3);
        expect(component.requestOptions).toEqual({
            page: 3,
            search: 'hose',
            sortBy: 'ID',
        });
    });
    it('should sort, resting page ', function () {
        component.requestOptions = { page: 2, search: 'hose', sortBy: 'ID' };
        component.sortChanged('!ID');
        expect(component.requestOptions).toEqual({
            page: undefined,
            search: 'hose',
            sortBy: '!ID',
        });
    });
});
//# sourceMappingURL=base-browse.class.spec.js.map