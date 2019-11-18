import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierUserDetailsComponent } from './supplier-user-details.component';

describe('SupplierUserDetailsComponent', () => {
  let component: SupplierUserDetailsComponent;
  let fixture: ComponentFixture<SupplierUserDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SupplierUserDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierUserDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
