import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SellerUserDetailsComponent } from './seller-user-details.component';

describe('SellerUserDetailsComponent', () => {
  let component: SellerUserDetailsComponent;
  let fixture: ComponentFixture<SellerUserDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SellerUserDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SellerUserDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
