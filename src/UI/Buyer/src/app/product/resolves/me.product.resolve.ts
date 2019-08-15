import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Params } from '@angular/router';
import { OcMeService, ListBuyerProduct, BuyerProduct, ListBuyerSpec, BuyerSpec, ListCategory } from '@ordercloud/angular-sdk';
import { each as _each } from 'lodash';
import { Observable, of } from 'rxjs';
import { FavoriteProductsService } from '@app-buyer/shared/services/favorites/favorites.service';

@Injectable()
export class MeListRelatedProductsResolver implements Resolve<ListBuyerProduct> {
  constructor(private service: OcMeService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Array<BuyerProduct>> | Promise<Array<BuyerProduct>> | any {
    const product = route.parent.data.product as BuyerProduct;
    if (!product.xp || !product.xp.RelatedProducts) {
      return of([]);
    }
    const calls: Array<Promise<BuyerProduct>> = new Array<Promise<BuyerProduct>>();
    product.xp.RelatedProducts.forEach((id: string) => {
      calls.push(this.service.GetProduct(id).toPromise());
    });
    return Promise.all(calls);
  }
}

@Injectable()
export class MeListProductResolver implements Resolve<ListBuyerProduct> {
  constructor(private service: OcMeService, private favoriteProductsService: FavoriteProductsService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<ListBuyerProduct> | Promise<ListBuyerProduct> | any {
    return this.service.ListProducts({
      categoryID: route.queryParams.category,
      page: route.queryParams.page,
      search: route.queryParams.search,
      sortBy: route.queryParams.sortBy,
      filters: {
        ...this.buildFacets(route.queryParams),
        ...this.buildFavoriesFilter(route.queryParams),
        ...this.buildPriceFilter(route.queryParams),
      },
    });
  }

  private buildFacets(params: Params): Params {
    const facets: Params = {};
    _each(params, (value, key) => {
      if (key !== 'page' && key !== 'search' && key !== 'sortBy' && key !== 'category' && key !== 'favoriteProducts') {
        facets[`xp.Facets.${key.toLocaleLowerCase()}`] = value;
      }
    });
    return facets;
  }

  private buildFavoriesFilter(params: Params): Params {
    const filter = {};
    const favorites = this.favoriteProductsService.getFavorites();
    filter['ID'] = params.favoriteProducts === 'true' && favorites ? favorites.join('|') : undefined;
    return filter;
  }

  private buildPriceFilter(params: Params): Params {
    const filter = {};
    if (params.minPrice && !params.maxPrice) {
      filter['xp.Price'] = `>=${params.minPrice}`;
    }
    if (params.maxPrice && !params.minPrice) {
      filter['xp.Price'] = `<=${params.maxPrice}`;
    }
    if (params.minPrice && params.maxPrice) {
      filter['xp.Price'] = [`>=${params.minPrice}`, `<=${params.maxPrice}`];
    }
    return filter;
  }
}

@Injectable()
export class MeProductResolver implements Resolve<BuyerProduct> {
  constructor(private service: OcMeService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<BuyerProduct> | Promise<BuyerProduct> | any {
    return this.service.GetProduct(route.params.productID);
  }
}

@Injectable()
export class MeListSpecsResolver implements Resolve<ListBuyerSpec> {
  constructor(private service: OcMeService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<ListBuyerSpec> | Promise<ListBuyerSpec> | any {
    return this.service.ListSpecs(route.params.productID);
  }
}

@Injectable()
export class MeSpecsResolver implements Resolve<BuyerSpec> {
  constructor(private service: OcMeService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<BuyerSpec> | Promise<BuyerSpec> | any {
    const list = route.parent.data.specList as ListBuyerSpec;
    const productID = route.parent.params.productID;
    const calls: Array<Promise<BuyerSpec>> = new Array<Promise<BuyerSpec>>();
    list.Items.forEach((item: BuyerSpec) => {
      calls.push(this.service.GetSpec(productID, item.ID).toPromise());
    });
    return Promise.all(calls);
  }
}

@Injectable()
export class MeListCategoriesResolver implements Resolve<ListCategory> {
  constructor(private service: OcMeService) {}

  resolve(): Observable<ListCategory> | Promise<ListCategory> | any {
    return this.service.ListCategories({ depth: 'all' });
  }
}
