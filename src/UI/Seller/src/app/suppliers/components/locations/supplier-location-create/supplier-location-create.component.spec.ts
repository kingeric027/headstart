import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierLocationCreateComponent } from './supplier-location-create.component';

describe('SupplierLocationCreateComponent', () => {
  let component: SupplierLocationCreateComponent;
  let fixture: ComponentFixture<SupplierLocationCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SupplierLocationCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierLocationCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
