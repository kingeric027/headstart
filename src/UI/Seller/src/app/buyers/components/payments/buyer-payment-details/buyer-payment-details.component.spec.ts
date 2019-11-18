import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerPaymentDetailsComponent } from './buyer-payment-details.component';

describe('BuyerPaymentDetailsComponent', () => {
  let component: BuyerPaymentDetailsComponent;
  let fixture: ComponentFixture<BuyerPaymentDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerPaymentDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerPaymentDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
