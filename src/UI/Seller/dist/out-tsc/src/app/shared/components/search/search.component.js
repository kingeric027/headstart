import { __decorate, __metadata } from "tslib";
import { Component, Input, EventEmitter, Output } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { faSearch, faTimes } from '@fortawesome/free-solid-svg-icons';
import { debounceTime, takeWhile, filter } from 'rxjs/operators';
var SearchComponent = /** @class */ (function () {
    function SearchComponent() {
        this.alive = true;
        this.searchTermInput = '';
        this.id = '';
        this.searched = new EventEmitter();
        this.faSearch = faSearch;
        this.faTimes = faTimes;
        this.previousSearchTerm = '';
    }
    SearchComponent.prototype.ngOnInit = function () {
        this.buildForm();
    };
    SearchComponent.prototype.ngOnChanges = function () {
        this.previousSearchTerm = this.searchTermInput;
        if (this.searchTermInput !== null && this.searchTermInput !== undefined && this.form) {
            this.form.setValue({ search: this.searchTermInput });
        }
    };
    SearchComponent.prototype.buildForm = function () {
        this.form = new FormGroup({ search: new FormControl(this.searchTermInput) });
        this.onFormChanges();
    };
    SearchComponent.prototype.onFormChanges = function () {
        var _this = this;
        this.form.controls.search.valueChanges
            .pipe(filter(function (searchTerm) {
            var userTriggered = _this.form.dirty;
            return searchTerm !== _this.previousSearchTerm && userTriggered;
        }), debounceTime(500), takeWhile(function () { return _this.alive; }))
            .subscribe(function (searchTerm) {
            _this.previousSearchTerm = searchTerm;
            _this.search();
        });
    };
    SearchComponent.prototype.search = function () {
        this.form.markAsPristine();
        // emit as undefined if empty string so sdk ignores parameter completely
        this.searched.emit(this.getCurrentSearchTerm() || undefined);
    };
    SearchComponent.prototype.getCurrentSearchTerm = function () {
        return this.form.get('search').value;
    };
    SearchComponent.prototype.showClear = function () {
        return this.getCurrentSearchTerm() !== '';
    };
    SearchComponent.prototype.clear = function () {
        this.form.markAsDirty();
        this.form.setValue({ search: '' });
    };
    SearchComponent.prototype.ngOnDestroy = function () {
        this.alive = false;
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], SearchComponent.prototype, "placeholderText", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], SearchComponent.prototype, "searchTermInput", void 0);
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], SearchComponent.prototype, "id", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], SearchComponent.prototype, "searched", void 0);
    SearchComponent = __decorate([
        Component({
            selector: 'search-component',
            templateUrl: './search.component.html',
            styleUrls: ['./search.component.scss'],
        })
    ], SearchComponent);
    return SearchComponent;
}());
export { SearchComponent };
//# sourceMappingURL=search.component.js.map