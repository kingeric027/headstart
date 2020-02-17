import { async, TestBed } from '@angular/core/testing';
import { CategoryDetailsComponent } from './category-details.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { OcCategoryService } from '@ordercloud/angular-sdk';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, of } from 'rxjs';
describe('CategoryDetailsComponent', function () {
    var component;
    var fixture;
    var mockCategory = { ID: 'myCategoryID' };
    var ocCategoryService = {
        Get: jasmine.createSpy('Get').and.returnValue(of({})),
        Patch: jasmine.createSpy('Patch').and.returnValue(of({})),
    };
    var activatedRoute = {
        params: new BehaviorSubject({ categoryID: 'myCategoryID' }),
    };
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [CategoryDetailsComponent],
            imports: [RouterTestingModule, FontAwesomeModule],
            providers: [
                { provide: OcCategoryService, useValue: ocCategoryService },
                { provide: applicationConfiguration, useValue: { buyerID: 'buyerID' } },
                { provide: ActivatedRoute, useValue: activatedRoute },
            ],
            schemas: [NO_ERRORS_SCHEMA],
        }).compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(CategoryDetailsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
    describe('ngOnInit', function () {
        beforeEach(function () {
            spyOn(component, 'getCategoryData').and.returnValue(of(mockCategory));
            component.ngOnInit();
        });
        it('should set category', function () {
            expect(component.getCategoryData).toHaveBeenCalled();
        });
    });
    describe('GetCategoryData', function () {
        it('should call OcCategoryService and set categoryID', function () {
            component.getCategoryData();
            expect(component.categoryID).toEqual(mockCategory.ID);
            expect(ocCategoryService.Get).toHaveBeenCalled();
        });
    });
    describe('updateProduct', function () {
        it('should update using existing categoryID', function () {
            var mock = { ID: 'newID' };
            component.updateCategory(mock);
            expect(ocCategoryService.Patch).toHaveBeenCalledWith(component.catalogID, component.categoryID, mock);
        });
    });
});
//# sourceMappingURL=category-details.component.spec.js.map