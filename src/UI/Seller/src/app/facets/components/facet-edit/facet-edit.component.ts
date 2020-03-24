import { Component, Input, Output, EventEmitter } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
import { FacetService } from '@app-seller/facets/facet.service';
import { faTimesCircle } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-facet-edit',
  templateUrl: './facet-edit.component.html',
  styleUrls: ['./facet-edit.component.scss'],
})
export class FacetEditComponent {
  @Input()
  resourceForm: FormGroup;
  @Input()
  filterConfig;
  @Input()
  resourceInSelection;
  @Input()
  updatedResource;
  @Output()
  updateResource = new EventEmitter<any>();
  isCreatingNew: boolean;
  faTimesCircle = faTimesCircle;
  constructor(public facetService: FacetService, private toaster: ToastrService) {
    this.isCreatingNew = this.facetService.checkIfCreatingNew();
  }
  updateResourceFromEvent(event: any, field: string): void {
    field === 'Active'
      ? this.updateResource.emit({ value: event.target.checked, field })
      : this.updateResource.emit({ value: event.target.value, field });
  }

  removeFacetOption(option: string): void {
    let copiedResource = this.facetService.copyResource(this.updatedResource);
    copiedResource.xp.Options = copiedResource.xp.Options.filter(o => o !== option);
    const event = {
      target: {
        value: copiedResource.xp.Options,
      },
    };
    this.updateResourceFromEvent(event, 'xp.Options');
  }

  addFacetOption() {
    const newFacetOptionInput = document.getElementById('newFacetOption') as any;
    let copiedResource = this.facetService.copyResource(this.updatedResource);
    // If facet options are null or undefined, initialize as an empty array
    if (copiedResource.xp.Options === null) copiedResource.xp.Options = [];
    // If proposed facet option already exists in the array, return warning
    if (copiedResource.xp.Options.includes(newFacetOptionInput.value)) {
      return this.toaster.warning(`The option "${newFacetOptionInput.value}" already exists.`);
    }
    copiedResource.xp.Options = copiedResource.xp.Options.concat(newFacetOptionInput.value);
    newFacetOptionInput.value = '';
    this.updateResourceFromEvent({ target: { value: copiedResource.xp.Options } }, 'xp.Options');
  }

  facetOptionLimitReached(): boolean {
      return this.updatedResource?.xp?.Options?.length === 25;
  }
}
