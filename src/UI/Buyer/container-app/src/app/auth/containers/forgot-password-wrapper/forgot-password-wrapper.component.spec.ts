import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ForgotPasswordWrapperComponent } from './forgot-password-wrapper.component';

describe('ForgotPasswordWrapperComponent', () => {
  let component: ForgotPasswordWrapperComponent;
  let fixture: ComponentFixture<ForgotPasswordWrapperComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ForgotPasswordWrapperComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ForgotPasswordWrapperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
