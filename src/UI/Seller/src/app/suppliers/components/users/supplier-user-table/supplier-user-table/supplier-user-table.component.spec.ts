import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { SupplierUserTableComponent } from '../supplier-table/supplier-user-table.component';

describe('SupplierTableComponent', () => {
  let component: SupplierUserTableComponent;
  let fixture: ComponentFixture<SupplierUserTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SupplierUserTableComponent],
    }).compileComponents();
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
