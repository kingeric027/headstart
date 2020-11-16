import { BuyerEditComponent } from './../../buyers/buyer-edit/buyer-edit.component';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

describe('BuyerCategoryEdit', () => {
  let component: BuyerEditComponent;
  let fixture: ComponentFixture<BuyerEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({ declarations: [BuyerEditComponent] }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
