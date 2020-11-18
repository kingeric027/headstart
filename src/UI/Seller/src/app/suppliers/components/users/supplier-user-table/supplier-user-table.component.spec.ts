import { SupplierUserTableComponent } from './supplier-user-table.component';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

describe('SupplierUserTableComponent', () => {
  let component: SupplierUserTableComponent;
  let fixture: ComponentFixture<SupplierUserTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({ declarations: [SupplierUserTableComponent] }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierUserTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
