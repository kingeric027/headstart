import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerCreateComponent } from './buyer-create.component';

describe('BuyerCreateComponent', () => {
  let component: BuyerCreateComponent;
  let fixture: ComponentFixture<BuyerCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
