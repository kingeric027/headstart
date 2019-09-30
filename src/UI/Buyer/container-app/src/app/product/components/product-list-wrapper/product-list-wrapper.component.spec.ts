import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductListWrapperComponent } from './product-list-wrapper.component';

describe('ProductListWrapperComponent', () => {
  let component: ProductListWrapperComponent;
  let fixture: ComponentFixture<ProductListWrapperComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductListWrapperComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductListWrapperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
