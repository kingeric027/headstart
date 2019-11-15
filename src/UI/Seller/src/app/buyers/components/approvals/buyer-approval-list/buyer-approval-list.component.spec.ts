import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerApprovalListComponent } from './buyer-approval-list.component';

describe('BuyerApprovalListComponent', () => {
  let component: BuyerApprovalListComponent;
  let fixture: ComponentFixture<BuyerApprovalListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerApprovalListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerApprovalListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
