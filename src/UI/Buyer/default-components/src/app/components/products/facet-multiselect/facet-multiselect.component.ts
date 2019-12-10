import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { ListFacet, ListFacetValue } from '@ordercloud/angular-sdk';
import { each as _each, get as _get, xor as _xor } from 'lodash';
import { faPlusSquare, faMinusSquare } from '@fortawesome/free-solid-svg-icons';
import { ProductFilters, ShopperContextService } from 'marketplace';
import { takeWhile } from 'rxjs/operators';

@Component({
  templateUrl: './facet-multiselect.component.html',
  styleUrls: ['./facet-multiselect.component.scss'],
})
export class OCMFacetMultiSelect implements OnDestroy {
  _facet: ListFacet;
  alive = true;
  checkboxArray: { facet: ListFacetValue; checked: boolean }[] = [];
  private activeFacetValues: string[] = [];
  visibleFacetLength = 5;
  isCollapsed = false;
  faPlusSquare = faPlusSquare;
  faMinusSquare = faMinusSquare;

  constructor(private context: ShopperContextService) {}

  @Input() set facet(value: ListFacet) {
    this._facet = value;
    this.context.productFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange);
  }

  toggleCollapsed() {
    this.isCollapsed = !this.isCollapsed;
    if (this.isCollapsed) this.visibleFacetLength = 5;
  }

  showAll() {
    this.visibleFacetLength = this._facet.Values.length;
  }

  handleCheckBoxClick(facetValue: string) {
    // remove if exists, add if doesn't
    this.activeFacetValues = _xor(this.activeFacetValues, [facetValue]);
    this.updateFilter(this.activeFacetValues);
  }

  private handleFiltersChange = (filters: ProductFilters) => {
    const activeFacet = _get(filters.activeFacets, this._facet.Name, null);
    this.activeFacetValues = activeFacet ? activeFacet.split('|') : [];
    this.updateCheckBoxes(this.activeFacetValues);
  }

  // TODO - there is this little flash when a checkbox is click. get rid of it.
  private updateCheckBoxes(activeFacetValues: string[]) {
    this.checkboxArray = this._facet.Values.map(facet => {
      const checked = activeFacetValues.includes(facet.Value);
      return { facet, checked };
    });
  }

  private updateFilter(activeFacetValues: string[]) {
    // TODO - maybe all this joining and spliting should be done in the service?
    // Abstract out the way the filters work under the hood?
    const values = activeFacetValues.join('|');
    this.context.productFilters.filterByFacet(this._facet.Name, values);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
