import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MeChangePasswordWrapperComponent } from './me-change-password-wrapper.component';

describe('MeChangePasswordWrapperComponent', () => {
  let component: MeChangePasswordWrapperComponent;
  let fixture: ComponentFixture<MeChangePasswordWrapperComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MeChangePasswordWrapperComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MeChangePasswordWrapperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
