import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Product, OcProductFacetService, ProductFacet } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';

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

  constructor(router: Router, activatedRoute: ActivatedRoute, private ocFacetService: OcProductFacetService) {
    super(router, activatedRoute, ocFacetService, '/facets', 'facets');
  }
}
