import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResetPasswordWrapperComponent } from './reset-password-wrapper.component';

describe('ResetPasswordWrapperComponent', () => {
  let component: ResetPasswordWrapperComponent;
  let fixture: ComponentFixture<ResetPasswordWrapperComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResetPasswordWrapperComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResetPasswordWrapperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
