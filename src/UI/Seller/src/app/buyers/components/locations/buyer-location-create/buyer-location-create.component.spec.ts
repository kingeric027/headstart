import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerLocationCreateComponent } from './buyer-location-create.component';

describe('BuyerLocationCreateComponent', () => {
  let component: BuyerLocationCreateComponent;
  let fixture: ComponentFixture<BuyerLocationCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerLocationCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerLocationCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
