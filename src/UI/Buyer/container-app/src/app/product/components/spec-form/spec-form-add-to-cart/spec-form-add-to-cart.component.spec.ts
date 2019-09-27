import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SpecFormAddToCartComponent } from './spec-form-add-to-cart.component';

describe('SpecFormButtonComponent', () => {
  let component: SpecFormAddToCartComponent;
  let fixture: ComponentFixture<SpecFormAddToCartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SpecFormAddToCartComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SpecFormAddToCartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
