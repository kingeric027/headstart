import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
import { SupplierService } from '../supplier.service';

import { SupplierTableComponent } from './supplier-table.component';

fdescribe('SupplierTableComponent', () => {
  let component: SupplierTableComponent;
  let fixture: ComponentFixture<SupplierTableComponent>;

  const router = { navigateByUrl: jasmine.createSpy('navigateByUrl') };
  const activatedRoute = { snapshot: { queryParams: {} } };
  const supplierService = {};
  const middlewareApiService = {};
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SupplierTableComponent],
      imports: [HttpClientModule],
      providers: [
        { provide: Router, useValue: router },
        { provide: ActivatedRoute, useValue: activatedRoute },
        { provide: SupplierService, useValue: supplierService },
        { provide: MiddlewareAPIService, useValue: middlewareApiService },
      ],
    }).compileComponents();
    fixture = TestBed.createComponent(SupplierTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
