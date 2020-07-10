import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Product, OcProductFacetService, ProductFacet } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';

@Injectable({
  providedIn: 'root',
})
export class FacetService extends ResourceCrudService<ProductFacet> {
  emptyResource = {
    ID: '',
    Name: '',
    xp: {
      Options: null,
    },
  };

  constructor(router: Router, activatedRoute: ActivatedRoute, private ocFacetService: OcProductFacetService, currentUserService: CurrentUserService, middleware: MiddlewareAPIService,) {
    super(router, activatedRoute, ocFacetService, currentUserService, middleware, '/facets', 'facets');
  }
}
