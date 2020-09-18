import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceBreadcrumbsComponent } from './resource-breadcrumbs.component';

describe('ResourceBreadcrumbsComponent', () => {
  let component: ResourceBreadcrumbsComponent;
  let fixture: ComponentFixture<ResourceBreadcrumbsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResourceBreadcrumbsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceBreadcrumbsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
