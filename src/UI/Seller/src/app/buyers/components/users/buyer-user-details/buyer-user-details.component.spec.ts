import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerUserDetailsComponent } from './buyer-user-details.component';

describe('BuyerUserDetailsComponent', () => {
  let component: BuyerUserDetailsComponent;
  let fixture: ComponentFixture<BuyerUserDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerUserDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerUserDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
