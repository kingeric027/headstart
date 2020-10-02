import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { OcProductFacetService, ProductFacet } from '@ordercloud/angular-sdk';
import { omit as _omit } from 'lodash';
import { faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import { cloneDeep } from 'lodash';


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
    this.facetsOnProductStatic = cloneDeep(facets);
    this.facetsOnProductEditable = facets;
    console.log('facets at this point', this.facetsOnProductStatic);
    console.log('facets at this point 2', this.facetsOnProductEditable);
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
    console.log('productXpFacetKey', productXpFacetKey);
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
    if (this.checkForFacetOverrides()) {
      this.overriddenChanges = true;
      console.log('static', this.facetsOnProductStatic)
      console.log('editable', this.facetsOnProductEditable)
    } else {
      console.log('should be hitting this condition');
      this.overriddenChanges = false;
    }
  }

  toggleSellerFilterOverride(): void {
    this.overriddenChanges = false;
    this.sellerFilterOverride = !this.sellerFilterOverride;
    if (!this.sellerFilterOverride) {
      this.facetsOnProductEditable = cloneDeep(this.facetsOnProductStatic);
    }
  }

  checkForFacetOverrides(): boolean {
    const keys = Object.keys(this.facetsOnProductStatic);
    let changeDetected = false;
    keys.forEach(key => {
      if (this.facetsOnProductEditable[key].length !== this.facetsOnProductStatic[key].length ||
          !this.facetsOnProductEditable[key].every(item => this.facetsOnProductStatic[key].includes(item))) {
        changeDetected = true;
      }
    });
    console.log('change detected?', changeDetected);
    return changeDetected;
  }

  saveFilterOverrides(): Promise<any> {
    
  }
}
