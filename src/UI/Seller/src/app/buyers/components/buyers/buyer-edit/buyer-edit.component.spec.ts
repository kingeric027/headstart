import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierTableComponent } from './buyer-edit.component';

describe('SupplierTableComponent', () => {
  let component: SupplierTableComponent;
  let fixture: ComponentFixture<SupplierTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SupplierTableComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
