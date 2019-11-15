import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerLocationListComponent } from './buyer-location-list.component';

describe('BuyerLocationListComponent', () => {
  let component: BuyerLocationListComponent;
  let fixture: ComponentFixture<BuyerLocationListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerLocationListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerLocationListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
