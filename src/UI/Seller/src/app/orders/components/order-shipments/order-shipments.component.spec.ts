import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderShipmentsComponent } from './order-shipments.component';

describe('OrderShipmentsComponent', () => {
  let component: OrderShipmentsComponent;
  let fixture: ComponentFixture<OrderShipmentsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrderShipmentsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderShipmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
