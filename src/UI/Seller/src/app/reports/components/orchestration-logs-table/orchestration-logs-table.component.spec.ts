import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrchestrationLogsTableComponent } from './orchestration-logs-table.component';

describe('OrchestrationLogsTableComponent', () => {
  let component: OrchestrationLogsTableComponent;
  let fixture: ComponentFixture<OrchestrationLogsTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrchestrationLogsTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrchestrationLogsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
