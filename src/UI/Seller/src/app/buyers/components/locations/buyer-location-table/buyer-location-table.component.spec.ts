import { BuyerLocationTableComponent } from './buyer-location-table.component';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

describe('BuyerLocationTable', () => {
  let component: BuyerLocationTableComponent;
  let fixture: ComponentFixture<BuyerLocationTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({ declarations: [BuyerLocationTableComponent] }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerLocationTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
