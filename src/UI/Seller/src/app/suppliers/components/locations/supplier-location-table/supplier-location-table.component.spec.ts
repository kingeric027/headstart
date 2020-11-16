import { SupplierLocationTableComponent } from './supplier-location-table.component';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

describe('SupplierLocationTableComponent', () => {
  let component: SupplierLocationTableComponent;
  let fixture: ComponentFixture<SupplierLocationTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({ declarations: [SupplierLocationTableComponent] }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierLocationTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
