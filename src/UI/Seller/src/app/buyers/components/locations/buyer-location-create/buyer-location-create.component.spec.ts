import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerUserCreateComponent } from './buyer-location-create.component';

describe('BuyerUserCreateComponent', () => {
  let component: BuyerUserCreateComponent;
  let fixture: ComponentFixture<BuyerUserCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerUserCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerUserCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
