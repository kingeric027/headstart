import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SellerUserCreateComponent } from './seller-user-create.component';

describe('SellerUserCreateComponent', () => {
  let component: SellerUserCreateComponent;
  let fixture: ComponentFixture<SellerUserCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SellerUserCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SellerUserCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
