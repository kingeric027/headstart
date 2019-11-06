import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerApprovalDetailsComponent } from './buyer-approval-details.component';

describe('BuyerApprovalDetailsComponent', () => {
  let component: BuyerApprovalDetailsComponent;
  let fixture: ComponentFixture<BuyerApprovalDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerApprovalDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerApprovalDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
