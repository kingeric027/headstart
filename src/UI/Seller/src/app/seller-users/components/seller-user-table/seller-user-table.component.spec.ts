import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SellerUserTableComponent } from './seller-user-table.component';

describe('SellerUserListComponent', () => {
  let component: SellerUserTableComponent;
  let fixture: ComponentFixture<SellerUserTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({ declarations: [SellerUserTableComponent] }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SellerUserTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
