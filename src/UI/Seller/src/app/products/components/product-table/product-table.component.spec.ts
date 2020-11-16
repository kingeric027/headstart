import { ProductTableComponent } from './product-table.component';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

describe('ProductTableComponent', () => {
  let component: ProductTableComponent;
  let fixture: ComponentFixture<ProductTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({ declarations: [ProductTableComponent] }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
