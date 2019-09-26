import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MeUpdateWrapperComponent } from './me-update-wrapper.component';

describe('MeUpdateWrapperComponent', () => {
  let component: MeUpdateWrapperComponent;
  let fixture: ComponentFixture<MeUpdateWrapperComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MeUpdateWrapperComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MeUpdateWrapperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
