import { Component, Input, OnDestroy } from '@angular/core';
import { ListFacet, ListFacetValue } from 'ordercloud-javascript-sdk';
import { get as _get, xor as _xor } from 'lodash';
import { faPlusSquare, faMinusSquare } from '@fortawesome/free-solid-svg-icons';
import { ProductFilters, ShopperContextService } from 'marketplace';
import { takeWhile } from 'rxjs/operators';

@Component({
  templateUrl: './facet-multiselect.component.html',
  styleUrls: ['./facet-multiselect.component.scss'],
})
export class OCMFacetMultiSelect implements OnDestroy {
  _facet: ListFacet;
  facetID: string;
  alive = true;
  checkboxArray: { facet: ListFacetValue; checked: boolean }[] = [];
  visibleFacetLength = 5;
  isCollapsed = false;
  faPlusSquare = faPlusSquare;
  faMinusSquare = faMinusSquare;

  private activeFacetValues: string[] = [];

  constructor(private context: ShopperContextService) {}

  @Input() set facet(value: ListFacet) {
    this._facet = value;
    this.facetID = this._facet.XpPath.split('.')[1];
    this.context.productFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
  }

  toggleCollapsed(): void {
    this.isCollapsed = !this.isCollapsed;
    if (this.isCollapsed) this.visibleFacetLength = 5;
  }

  showAll(): void {
    this.visibleFacetLength = this._facet.Values.length;
  }

  handleCheckBoxClick(facetValue: string): void {
    // remove if exists, add if doesn't
    this.activeFacetValues = _xor(this.activeFacetValues, [facetValue]);
    this.updateFilter(this.activeFacetValues);
  }

  ngOnDestroy(): void {
    this.alive = false;
  }

  private handleFiltersChange = (filters: ProductFilters): void => {
    const activeFacet = _get(filters.activeFacets, this.facetID, null);
    this.activeFacetValues = activeFacet ? activeFacet.split('|') : [];
    this.updateCheckBoxes(this.activeFacetValues);
  };

  // TODO - there is this little flash when a checkbox is click. get rid of it.
  private updateCheckBoxes(activeFacetValues: string[]): void {
    this.checkboxArray = this._facet.Values.map(facet => {
      const checked = activeFacetValues.includes(facet.Value);
      return { facet, checked };
    });
  }

  private updateFilter(activeFacetValues: string[]): void {
    // TODO - maybe all this joining and spliting should be done in the service?
    // Abstract out the way the filters work under the hood?
    const values = activeFacetValues.join('|');
    this.context.productFilters.filterByFacet(this.facetID, values);
  }
}
