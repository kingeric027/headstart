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
  sellerFilterOverride: boolean;
  facetsOnProductStatic: any[];
  facetsOnProductEditable: any[];
  overriddenChanges: boolean;

  @Input() set facetsOnProduct (facets: any[]) {
    this.facetsOnProductStatic = facets;
    this.facetsOnProductEditable = facets;
  };
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
    console.log('confirm the facets on init', facets);
    this.facetOptions = facets.Items.filter(f => f?.xp?.Options?.length);
    console.log('confirm the facet options on init', this.facetOptions);
  }

  areFacetOptionsSelected(facet: ProductFacet): boolean {
    const productXpFacetKey = facet?.XpPath.split('.')[1];
    return Object.keys(this.facetsOnProductEditable).includes(productXpFacetKey);
  }

  isFacetOptionApplied(facet: ProductFacet, option: string): boolean {
    const productXpFacetKey = facet?.XpPath.split('.')[1];
    const facetOptionsOnProduct = this.facetsOnProductEditable[productXpFacetKey];
    const isFacetOptionApplied = facetOptionsOnProduct && facetOptionsOnProduct.includes(option);
    return isFacetOptionApplied;
  }

  toggleFacetOption(facet: ProductFacet, option: string): void {
    console.log('facet', facet);
    console.log('option', option);
    console.log('facetsOnProduct', this.facetsOnProductEditable);
    const productXpFacetKey = facet?.XpPath.split('.')[1]; //Color
    let facetOnXp = this.facetsOnProductEditable[productXpFacetKey]; //What is currently selected...*selects green* ['White', 'Orange'], *selects purple* ['White', 'Orange', 'Green']
    console.log('facetOnXp', facetOnXp);
    delete this.facetsOnProductEditable[productXpFacetKey]; //delete removes a property from an object
    // If the key doesn't exist on Product.xp.Facets, initialize it as an empty array
    if (!facetOnXp) {
      facetOnXp = [] 
    } //don't think this condition will come up
    
    // If the facet in quetsion includes the option requested, remove it from the array, else add it.
    if(facetOnXp.includes(option)) {
      console.log('WE HAVE THIS, DELETE!');
      facetOnXp = facetOnXp.filter(o => o !== option);
    } else {
      console.log('WE DO NOT HAVE THIS, ADD!');
      facetOnXp.push(option);
    }
    
    if (facetOnXp.length > 0) {
      this.facetsOnProductEditable = { ...this.facetsOnProductEditable, [productXpFacetKey]: facetOnXp}
    };
    // this.updatedFacets.emit(this.facetsOnProduct); //COMMENTED OUT FOR NOW - UNCOMMENT LATER

    //TESTING
    if (!this.readonly) {
      this.updatedFacets.emit(this.facetsOnProductEditable);
    } else {
      this.overrideFacets();
    }
  }

  overrideFacets(): void {
    console.log('overriding facets', this.facetsOnProductEditable);
    if (this.facetsOnProductStatic != this.facetsOnProductEditable) {
      this.overriddenChanges = true;
    } else {
      this.overriddenChanges = false;
    }
  }

  toggleSellerFilterOverride(): void {
    this.sellerFilterOverride = !this.sellerFilterOverride;
  }
}
