import { Component, Input } from '@angular/core'
import { faAngleLeft, faAngleRight } from '@fortawesome/free-solid-svg-icons'
import { MarketplaceMeProduct } from 'marketplace'

@Component({
  templateUrl: './product-carousel.component.html',
  styleUrls: ['./product-carousel.component.scss'],
})
export class OCMProductCarousel {
  @Input() products: MarketplaceMeProduct[] = []
  @Input() displayTitle: string

  index = 0
  rowLength = 4
  faAngleLeft = faAngleLeft
  faAngleRight = faAngleRight

  left(): void {
    this.index -= this.rowLength
  }

  right(): void {
    this.index += this.rowLength
  }

  getProducts(): MarketplaceMeProduct[] {
    return this.products.slice(this.index, this.index + this.rowLength)
  }
}
