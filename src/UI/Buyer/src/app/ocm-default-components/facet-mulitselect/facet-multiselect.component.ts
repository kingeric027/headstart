import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { FormGroup, FormArray, FormControl } from '@angular/forms';
import { ListFacet } from '@ordercloud/angular-sdk';
import { each as _each, get as _get, xor as _xor } from 'lodash';
import { faPlusSquare, faMinusSquare } from '@fortawesome/free-solid-svg-icons';
import { ProductFilters } from '@app-buyer/shared/services/product-filter/product-filter.service';
import { OCMComponent } from '@app-buyer/ocm-default-components/shopper-context';

@Component({
  templateUrl: './facet-multiselect.component.html',
  styleUrls: ['./facet-multiselect.component.scss'],
})
export class OCMFacetMultiSelect extends OCMComponent implements OnInit, OnChanges {
  @Input() facet: ListFacet;
  form: FormGroup;
  private activeFacetValues: string[] = [];
  visibleFacetLength = 5;
  isCollapsed = false;
  faPlusSquare = faPlusSquare;
  faMinusSquare = faMinusSquare;

  ngOnInit() {
    debugger;
    this.form = new FormGroup({ facetValues: new FormArray([]) });
  }

  ngOnChanges() {
    debugger;
    if (!(<FormArray>this.form.get('facetValues')).value.length) this.buildForm();
    this.context.productFilterActions.onFiltersChange(this.handleFiltersChange);
  }

  toggleCollapsed() {
    this.isCollapsed = !this.isCollapsed;
    if (this.isCollapsed) this.visibleFacetLength = 5;
  }

  showAll() {
    this.visibleFacetLength = this.facet.Values.length;
  }

  handleCheckBoxClick(facetValue: string) {
    // remove if exists, add if doesn't
    this.activeFacetValues = _xor(this.activeFacetValues, [facetValue]);
    this.updateFilter(this.activeFacetValues);
  }

  private buildForm() {
    debugger;
    // initialize a blank form with all values set to false
    const checkboxes = this.facet.Values.map(() => new FormControl(false));
    this.form = new FormGroup({ facetValues: new FormArray(checkboxes) });
  }

  private handleFiltersChange = (filters: ProductFilters) => {
    const activeFacet = _get(filters.activeFacets, this.facet.Name, null);
    this.activeFacetValues = activeFacet ? activeFacet.split('|') : [];
    this.updateCheckBoxes(this.activeFacetValues);
  };

  private updateCheckBoxes(activeFacetValues: string[]) {
    debugger;
    const isChecked: boolean[] = this.facet.Values.map((value) => {
      return activeFacetValues.includes(value.Value);
    });
    (<FormArray>this.form.get('facetValues')).setValue(isChecked);
  }

  private updateFilter(activeFacetValues: string[]) {
    // TODO - maybe all this joining and spliting should be done in the service?
    // Abstrat away the way the filters work under the hood?
    const values = activeFacetValues.join('|');
    this.context.productFilterActions.filterByFacet(this.facet.Name, values);
  }
}
