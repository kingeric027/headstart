import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ListBuyerProduct } from '@ordercloud/angular-sdk';
import { faBullhorn } from '@fortawesome/free-solid-svg-icons';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';

@Component({
  selector: 'ocm-home-page',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class OCMHomePage {
  @Input() featuredProducts: ListBuyerProduct;
  @Input() quantityLimits: QuantityLimits[];
  @Input() favoriteProductIDs: string[];
  @Input() navigator: Navigator;
  @Output() setIsFavorite = new EventEmitter<{ isfavorite: boolean; productID: string }>();
  faBullhorn = faBullhorn;

  // TODO - this content may need to be managed externally somehow.
  annoucement = 'This is an announcements areas whose content gets displayed on Homepage of Buyer Site.';
  carouselSlides = [
    {
      URL: '../../../assets/carousel2.jpg',
      headerText: 'Carousel Image Two',
      bodyText: 'Welcome to the home page',
    },
    {
      URL: '../../../assets/carousel3.jpg',
      headerText: 'Carousel Image Three',
      bodyText: 'This is the third image',
    },
  ];
}
