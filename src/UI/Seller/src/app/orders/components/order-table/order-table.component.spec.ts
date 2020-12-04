import { ChangeDetectorRef, NgZone } from '@angular/core'
import { async, ComponentFixture, TestBed } from '@angular/core/testing'
import { Router, ActivatedRoute } from '@angular/router'
import { AppAuthService } from '@app-seller/auth'
import { OrderService } from '@app-seller/orders/order.service'
import { of } from 'rxjs'

import { OrderTableComponent } from './order-table.component'

describe('OrderTableComponent', () => {
  let component: OrderTableComponent
  let fixture: ComponentFixture<OrderTableComponent>

  const router = {}
  const orderService = { resourceSubject: of({}) }
  const changeDetectorRef = {}
  const activatedRoute = { queryParams: of({}), params: of({}) }
  const ngZone = {}
  const appAuthService = {}

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [OrderTableComponent],
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
    fixture = TestBed.createComponent(OrderTableComponent)
    component = fixture.componentInstance
    fixture.detectChanges()
  })
})
