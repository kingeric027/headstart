import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierLocationListComponent } from './supplier-location-list.component';

describe('SupplierLocationListComponent', () => {
  let component: SupplierLocationListComponent;
  let fixture: ComponentFixture<SupplierLocationListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SupplierLocationListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierLocationListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
