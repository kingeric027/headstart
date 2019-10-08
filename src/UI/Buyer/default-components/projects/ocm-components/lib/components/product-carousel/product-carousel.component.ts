import { Component, Input } from '@angular/core';
import { faAngleLeft, faAngleRight } from '@fortawesome/free-solid-svg-icons';
import { BuyerProduct } from '@ordercloud/angular-sdk';
import { QuantityLimits } from '../../models/quantity-limits';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './product-carousel.component.html',
  styleUrls: ['./product-carousel.component.scss'],
})
export class OCMProductCarousel extends OCMComponent {
  @Input() products: BuyerProduct[] = [];
  @Input() displayTitle: string;
  @Input() quantityLimits: QuantityLimits[];

  index = 0;
  rowLength = 4;
  faAngleLeft = faAngleLeft;
  faAngleRight = faAngleRight;

  ngOnContextSet() {}

  left(): void {
    this.index -= this.rowLength;
  }

  right(): void {
    this.index += this.rowLength;
  }

  getProducts(): BuyerProduct[] {
    return this.products.slice(this.index, this.index + this.rowLength);
  }
}
