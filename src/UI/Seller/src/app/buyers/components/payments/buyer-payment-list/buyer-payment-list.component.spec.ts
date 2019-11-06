import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerPaymentListComponent } from './buyer-payment-list.component';

describe('BuyerPaymentListComponent', () => {
  let component: BuyerPaymentListComponent;
  let fixture: ComponentFixture<BuyerPaymentListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerPaymentListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerPaymentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
