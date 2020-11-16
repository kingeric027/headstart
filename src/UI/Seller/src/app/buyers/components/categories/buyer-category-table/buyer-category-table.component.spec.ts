import { BuyerCategoryTableComponent } from './buyer-category-table.component';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

describe('BuyerCategoryTableComponent', () => {
  let component: BuyerCategoryTableComponent;
  let fixture: ComponentFixture<BuyerCategoryTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({ declarations: [BuyerCategoryTableComponent] }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerCategoryTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
