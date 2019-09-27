import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MeAddressListWrapperComponent } from './address-list-wrapper.component';

describe('AddressListWrapperComponent', () => {
  let component: MeAddressListWrapperComponent;
  let fixture: ComponentFixture<MeAddressListWrapperComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [MeAddressListWrapperComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MeAddressListWrapperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
