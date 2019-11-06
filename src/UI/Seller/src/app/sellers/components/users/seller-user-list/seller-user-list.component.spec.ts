import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SellerUserListComponent } from './seller-user-list.component';

describe('SellerUserListComponent', () => {
  let component: SellerUserListComponent;
  let fixture: ComponentFixture<SellerUserListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SellerUserListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SellerUserListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
