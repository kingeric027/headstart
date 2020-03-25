import { Component, ChangeDetectorRef, NgZone, OnInit } from '@angular/core';
import { ProductFacet } from '@ordercloud/angular-sdk';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { FacetService } from '@app-seller/facets/facet.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-facet-table',
  templateUrl: './facet-table.component.html',
  styleUrls: ['./facet-table.component.scss'],
})
export class FacetTableComponent extends ResourceCrudComponent<ProductFacet> implements OnInit {
  constructor(
    facetService: FacetService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedRoute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, facetService, router, activatedRoute, ngZone);
  }

  // static filters that should apply to all marketplace orgs, custom filters for specific applications can be
  // added to the filterconfig passed into the resourcetable in the future
  filterConfig = {
    Filters: [],
  };
}
