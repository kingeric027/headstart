import { BuyerPaymentTableComponent } from './buyer-payment-table.component';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

describe('BuyerPaymentTableComponent', () => {
  let component: BuyerPaymentTableComponent;
  let fixture: ComponentFixture<BuyerPaymentTableComponent>;
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [BuyerPaymentTableComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerPaymentTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
