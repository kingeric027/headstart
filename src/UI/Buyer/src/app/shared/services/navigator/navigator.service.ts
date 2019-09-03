import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

export interface Navigator {
  toProductDetails: (productID: string) => void;
  toProductList: () => void;
  toCheckout(): () => void;
}

@Injectable({
  providedIn: 'root',
})
export class NavigatorService {
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
