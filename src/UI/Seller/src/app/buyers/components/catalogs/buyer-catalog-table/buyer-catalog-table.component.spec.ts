import { BuyerCatalogTableComponent } from './buyer-catalog-table.component';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

describe('BuyerCatalogTableComponent', () => {
  let component: BuyerCatalogTableComponent;
  let fixture: ComponentFixture<BuyerCatalogTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({ declarations: [BuyerCatalogTableComponent] }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerCatalogTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
