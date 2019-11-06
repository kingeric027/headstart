import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyerUserListComponent } from './buyer-user-list.component';

describe('BuyerUserListComponent', () => {
  let component: BuyerUserListComponent;
  let fixture: ComponentFixture<BuyerUserListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyerUserListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyerUserListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
