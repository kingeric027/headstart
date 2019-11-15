import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierUserCreateComponent } from './supplier-user-create.component';

describe('SupplierUserCreateComponent', () => {
  let component: SupplierUserCreateComponent;
  let fixture: ComponentFixture<SupplierUserCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SupplierUserCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierUserCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
