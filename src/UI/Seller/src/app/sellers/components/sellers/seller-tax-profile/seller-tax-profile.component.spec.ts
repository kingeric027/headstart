import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SellerTaxProfileComponent } from './seller-tax-profile.component';

describe('SellerTaxProfileComponent', () => {
  let component: SellerTaxProfileComponent;
  let fixture: ComponentFixture<SellerTaxProfileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SellerTaxProfileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SellerTaxProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
