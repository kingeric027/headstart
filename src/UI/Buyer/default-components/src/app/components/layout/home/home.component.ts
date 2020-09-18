import { faBullhorn } from '@fortawesome/free-solid-svg-icons';
import { Component, OnInit } from '@angular/core';
import { ShopperContextService, MarketplaceMeProduct } from 'marketplace';
import { StaticPageService } from 'marketplace';

@Component({
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class OCMHomePage implements OnInit {
  featuredProducts: MarketplaceMeProduct[];
  faBullhorn = faBullhorn;
  URL = '../../../assets/jumbotron.svg';

  constructor(private context: ShopperContextService, public staticPageService: StaticPageService) { }

  async ngOnInit(): Promise<void> {
    const products = await this.context.tempSdk.listMeProducts({ filters: { 'xp.Featured': true } });
    this.featuredProducts = products.Items;
  }

  // TODO: add PageDocument type to cms library so this is strongly typed
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  get homePageDoc(): any {
    return this.staticPageService.pages.find(page => page.Doc.Url === 'home');
  }

  toSupplier(supplier: string): void {
    this.context.router.toProductList({ activeFacets: { Supplier: supplier.toLocaleLowerCase() } });
  }
}
