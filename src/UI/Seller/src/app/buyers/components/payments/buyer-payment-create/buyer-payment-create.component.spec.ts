import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerPaymentCreateComponent } from './buyer-payment-create.component';

describe('BuyerPaymentCreateComponent', () => {
  let component: BuyerPaymentCreateComponent;
  let fixture: ComponentFixture<BuyerPaymentCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerPaymentCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerPaymentCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
