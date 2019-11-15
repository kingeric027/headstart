import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierLocationDetailsComponent } from './supplier-location-details.component';

describe('SupplierLocationDetailsComponent', () => {
  let component: SupplierLocationDetailsComponent;
  let fixture: ComponentFixture<SupplierLocationDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SupplierLocationDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierLocationDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
