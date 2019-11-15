import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerLocationDetailsComponent } from './buyer-location-details.component';

describe('BuyerLocationDetailsComponent', () => {
  let component: BuyerLocationDetailsComponent;
  let fixture: ComponentFixture<BuyerLocationDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerLocationDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerLocationDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
