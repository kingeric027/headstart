import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerDetailsComponent } from './buyer-details.component';

describe('BuyerDetailsComponent', () => {
  let component: BuyerDetailsComponent;
  let fixture: ComponentFixture<BuyerDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
