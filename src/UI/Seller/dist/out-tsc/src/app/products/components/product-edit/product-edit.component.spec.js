import { async, TestBed } from '@angular/core/testing';
import { SupplierTableComponent } from './supplier-edit.component';
describe('SupplierTableComponent', function () {
    var component;
    var fixture;
    beforeEach(async(function () {
        TestBed.configureTestingModule({
            declarations: [SupplierTableComponent],
        }).compileComponents();
    }));
    beforeEach(function () {
        fixture = TestBed.createComponent(SupplierTableComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=product-edit.component.spec.js.map