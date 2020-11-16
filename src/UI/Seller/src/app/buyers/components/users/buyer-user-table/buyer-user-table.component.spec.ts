import { BuyerUserTableComponent } from './buyer-user-table.component';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

describe('BuyerUserTableComponent', () => {
  let component: BuyerUserTableComponent;
  let fixture: ComponentFixture<BuyerUserTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({ declarations: [BuyerUserTableComponent] }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerUserTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
