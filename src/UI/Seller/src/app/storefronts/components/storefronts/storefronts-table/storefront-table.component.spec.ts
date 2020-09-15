import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StorefrontTableComponent } from './storefront-table.component';

describe('StorefrontTableComponent', () => {
  let component: StorefrontTableComponent;
  let fixture: ComponentFixture<StorefrontTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [StorefrontTableComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StorefrontTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
