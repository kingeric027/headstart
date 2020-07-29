import { faBullhorn } from '@fortawesome/free-solid-svg-icons';
import { Component, OnInit } from '@angular/core';
import { ShopperContextService, MarketplaceMeProduct } from 'marketplace';
import { BuyerProduct } from 'ordercloud-javascript-sdk';
import { ProductXp } from '@ordercloud/headstart-sdk';

@Component({
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class OCMHomePage implements OnInit {
  featuredProducts: MarketplaceMeProduct[];
  faBullhorn = faBullhorn;
  URL = '../../../assets/jumbotron.svg';

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
  constructor(private context: ShopperContextService) {}

  async ngOnInit(): Promise<void> {
    const products = await this.context.tempSdk.listMeProducts({ filters: { 'xp.Featured': true } });
    this.featuredProducts = products.Items;
  }

  toSupplier(supplier: string): void {
    this.context.router.toProductList({ activeFacets: { Supplier: supplier.toLocaleLowerCase() } });
  }
}
