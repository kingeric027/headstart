import { ProductEditComponent } from './product-edit.component'
import { async, ComponentFixture, TestBed } from '@angular/core/testing'
import { ChangeDetectorRef, NgZone } from '@angular/core'
import { Router, ActivatedRoute } from '@angular/router'
import { AppAuthService } from '@app-seller/auth'
import { OrderService } from '@app-seller/orders/order.service'
import { of } from 'rxjs'

describe('ProductEditComponent', () => {
  let component: ProductEditComponent
  let fixture: ComponentFixture<ProductEditComponent>

  const router = {}
  const orderService = { resourceSubject: of({}) }
  const changeDetectorRef = {}
  const activatedRoute = { queryParams: of({}), params: of({}) }
  const ngZone = {}
  const appAuthService = {}

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ProductEditComponent],
      providers: [
        { provide: Router, useValue: router },
        { provide: OrderService, useValue: orderService },
        { provide: ChangeDetectorRef, useValue: changeDetectorRef },
        { provide: ActivatedRoute, useValue: activatedRoute },
        { provide: NgZone, useValue: ngZone },
        { provide: AppAuthService, useValue: appAuthService },
      ],
    }).compileComponents()
  }))

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductEditComponent)
    component = fixture.componentInstance
    fixture.detectChanges()
  })

  it('should create', () => {
    expect(component).toBeTruthy()
  })
})
