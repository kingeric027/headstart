import { Component, ChangeDetectorRef, NgZone, OnInit } from '@angular/core';
import { ProductFacet } from '@ordercloud/angular-sdk';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { FacetService } from '@app-seller/facets/facet.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

function createSupplierUserForm(facet: ProductFacet) {
  return new FormGroup({
    ID: new FormControl(facet.ID, Validators.required),
    Name: new FormControl(facet.Name, Validators.required),
    XpPath: new FormControl(facet.XpPath),
    Options: new FormControl(facet?.xp?.Options),
  });
}

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
    super(changeDetectorRef, facetService, router, activatedRoute, ngZone, createSupplierUserForm);
  }

  // static filters that should apply to all marketplace orgs, custom filters for specific applications can be
  // added to the filterconfig passed into the resourcetable in the future
  filterConfig = {
    Filters: [],
  };
}
