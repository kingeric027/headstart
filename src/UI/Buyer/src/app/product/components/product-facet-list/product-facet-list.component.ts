import { Component, Input } from '@angular/core';
import { ListFacet } from '@ordercloud/angular-sdk';
import { OCMComponent } from '@app-buyer/ocm-default-components/shopper-context';

@Component({
  selector: 'ocm-product-facet-list',
  templateUrl: './product-facet-list.component.html',
  styleUrls: ['./product-facet-list.component.scss'],
})
export class OCMProductFacetList extends OCMComponent {
  @Input() facetList: ListFacet[];
}
