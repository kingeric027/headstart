import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { RouteActions } from '@app-buyer/ocm-default-components/shopper-context';

@Injectable({
  providedIn: 'root',
})
export class RouteService implements RouteActions {
  constructor(private router: Router) {}

  toProductDetails(productID: string): void {
    this.router.navigateByUrl(`/products/${productID}`);
  }

  toProductList(): void {
    this.router.navigateByUrl('/products');
  }

  toCheckout(): void {
    this.router.navigateByUrl('/checkout');
  }
}
