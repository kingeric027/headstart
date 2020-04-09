import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { OcProductFacetService, ProductFacet } from '@ordercloud/angular-sdk';
import { omit as _omit } from 'lodash';
import { faCheckCircle } from '@fortawesome/free-solid-svg-icons';


@Component({
  selector: 'product-filters-component',
  templateUrl: './product-filters.component.html',
  styleUrls: ['./product-filters.component.scss'],
})
export class ProductFilters implements OnInit{
  facetOptions: ProductFacet[];
  faCheckCircle = faCheckCircle;

  @Input() facetsOnProduct: any = [];
  @Input() readonly = false;
  @Output() updatedFacets = new EventEmitter<any>();

  constructor(
    private ocFacetService: OcProductFacetService,
  ) {}

  ngOnInit(): void {
    this.getFacets();
  }

  async getFacets(): Promise<void> {
    const facets = await this.ocFacetService.List().toPromise();
    this.facetOptions = facets.Items.filter(f => f?.xp?.Options?.length);
  }

  areFacetOptionsSelected(facet: ProductFacet): boolean {
    const productXpFacetKey = facet?.XpPath.split('.')[1];
    return Object.keys(this.facetsOnProduct).includes(productXpFacetKey);
  }

  isFacetOptionApplied(facet: ProductFacet, option: string): boolean {
    const productXpFacetKey = facet?.XpPath.split('.')[1];
    const facetOptionsOnProduct = this.facetsOnProduct[productXpFacetKey];
    const isFacetOptionApplied = facetOptionsOnProduct && facetOptionsOnProduct.includes(option);
    return isFacetOptionApplied;
  }

  toggleFacetOption(facet: ProductFacet, option: string): void {
    const productXpFacetKey = facet?.XpPath.split('.')[1];
    let facetOnXp = this.facetsOnProduct[productXpFacetKey];
    delete this.facetsOnProduct[productXpFacetKey];
    // If the key doesn't exist on Product.xp.Facets, initialize it as an empty array
    if (!facetOnXp) {
      facetOnXp = [] 
    }
    
    // If the facet in quetsion includes the option requested, remove it from the array, else add it.
    if(facetOnXp.includes(option)) {
      facetOnXp = facetOnXp.filter(o => o !== option);
    } else {
      facetOnXp.push(option);
    }
    
    if (facetOnXp.length > 0) {
      this.facetsOnProduct = { ...this.facetsOnProduct, [productXpFacetKey]: facetOnXp}
    };
    this.updatedFacets.emit(this.facetsOnProduct);
  }
}
