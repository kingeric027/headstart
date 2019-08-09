import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SpecFormButtonComponent } from './spec-form-button.component';

describe('SpecFormButtonComponent', () => {
  let component: SpecFormButtonComponent;
  let fixture: ComponentFixture<SpecFormButtonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SpecFormButtonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SpecFormButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
