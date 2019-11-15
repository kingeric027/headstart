import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerApprovalsCreateComponent } from './buyer-approval-create.component';

describe('BuyerApprovalsCreateComponent', () => {
  let component: BuyerApprovalsCreateComponent;
  let fixture: ComponentFixture<BuyerApprovalsCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [BuyerApprovalsCreateComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerApprovalsCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
