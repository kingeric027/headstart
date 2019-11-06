import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SellerNotificationsComponent } from './seller-notifications.component';

describe('SellerNotificationsComponent', () => {
  let component: SellerNotificationsComponent;
  let fixture: ComponentFixture<SellerNotificationsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SellerNotificationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SellerNotificationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
