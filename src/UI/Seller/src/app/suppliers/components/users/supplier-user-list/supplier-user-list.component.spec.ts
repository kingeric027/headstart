import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierUserListComponent } from './supplier-user-list.component';

describe('SupplierUserListComponent', () => {
  let component: SupplierUserListComponent;
  let fixture: ComponentFixture<SupplierUserListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SupplierUserListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierUserListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
