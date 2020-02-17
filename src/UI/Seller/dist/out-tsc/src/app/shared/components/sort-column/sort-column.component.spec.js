import { async, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { SortColumnComponent } from '@app-seller/shared/components/sort-column/sort-column.component';
describe('SortTableHeaderComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [SortColumnComponent],
            schemas: [NO_ERRORS_SCHEMA],
        }).compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(SortColumnComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
    describe('changeSort', function () {
        beforeEach(function () {
            spyOn(component.sort, 'emit');
            component.fieldName = 'ID';
        });
        it('should sort accending if first click', function () {
            component.currentSort = undefined;
            component.changeSort();
            expect(component.sort.emit).toHaveBeenCalledWith('ID');
        });
        it('should sort descending if second click', function () {
            component.currentSort = 'ID';
            component.changeSort();
            expect(component.sort.emit).toHaveBeenCalledWith('!ID');
        });
        it('should clear sort if third click', function () {
            component.currentSort = '!ID';
            component.changeSort();
            expect(component.sort.emit).toHaveBeenCalledWith(undefined);
        });
    });
});
//# sourceMappingURL=sort-column.component.spec.js.map