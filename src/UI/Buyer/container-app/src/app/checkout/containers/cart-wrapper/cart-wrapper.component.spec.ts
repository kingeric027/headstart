import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CartWrapperComponent } from './cart-wrapper.component';

describe('CartWrapperComponent', () => {
  let component: CartWrapperComponent;
  let fixture: ComponentFixture<CartWrapperComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CartWrapperComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CartWrapperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
