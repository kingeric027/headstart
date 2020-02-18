import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { singular } from 'pluralize';
import {
  PRODUCT_IMAGE_PATH_STRATEGY,
  getProductMainImageUrlOrPlaceholder,
  PLACEHOLDER_URL,
} from '@app-seller/shared/services/product/product-image.helper';
import { OcCategoryService } from '@ordercloud/angular-sdk';
import {
  faChevronDown,
  faChevronUp
} from '@fortawesome/free-solid-svg-icons';
import { SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY } from '@app-seller/shared/services/configuration/table-display';

@Component({
  selector: 'summary-resource-display-component',
  templateUrl: './summary-resource-display.component.html',
  styleUrls: ['./summary-resource-display.component.scss'],
})
export class SummaryResourceDisplay implements OnChanges {
  _primaryHeader = '';
  _secondaryHeader = '';
  _imgPath = '';
  _shouldShowImage = false;
  _isNewPlaceHolder = false;
  _isExpandable = false;
  faChevronDown = faChevronDown;
  faChevronUp = faChevronUp;
  _isResourceExpanded: boolean;
  _resource: any;
  _resourceList: any;
  _resourceID: any = '';

  @Input()
  set resourceList(value: any) {
    this._resourceList = value;
  }
  @Input()
  resourceType: any;
  @Input()
  set isNewPlaceHolder(value: boolean) {
    this._isNewPlaceHolder = value;
  }
  @Input()
  resource: any;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.resourceType?.firstChange) {
      this.setDisplayValuesForPlaceholder();
    }
    if (changes.resource?.firstChange) {
      this.setDisplayValuesForResource(changes.resource.currentValue);
    }
  }
  @Input()
  set resourceID(value: any) {
    this._resourceID = value;
  }

  constructor(
    private ocCategoryService: OcCategoryService
    ) {}

  setDisplayValuesForResource(resource: any) {
    this._primaryHeader = this.getValueOnExistingResource(resource, 'toPrimaryHeader');
    this._secondaryHeader = this.getValueOnExistingResource(resource, 'toSecondaryHeader');
    this._shouldShowImage = !!SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY[this.resourceType]['toImage'];
    this._imgPath = this.getValueOnExistingResource(resource, 'toImage') || PLACEHOLDER_URL;
    this._isExpandable = resource.ChildCount > 0;   
  }

  getValueOnExistingResource(value: any, valueType: string) {
    const pathToValue = SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY[this.resourceType][valueType];
    const piecesOfPath = pathToValue.split('.');
    if (pathToValue) {
      if (pathToValue === PRODUCT_IMAGE_PATH_STRATEGY) {
        return getProductMainImageUrlOrPlaceholder(value);
      } else {
        let currentObject = value;
        piecesOfPath.forEach(piece => {
          currentObject = currentObject && currentObject[piece];
        });
        return currentObject;
      }
    } else {
      return '';
    }
  }

  setDisplayValuesForPlaceholder() {
    this._primaryHeader = this.getValueOnPlaceHolderResource('toPrimaryHeader');
    this._secondaryHeader = this.getValueOnPlaceHolderResource('toSecondaryHeader');
    this._imgPath = this.getValueOnPlaceHolderResource('toImage');
  }

  getValueOnPlaceHolderResource(valueType: string) {
    switch (valueType) {
      case 'toPrimaryHeader':
        return `Your new ${singular(this.resourceType)}`;
      default:
        return '';
    }
  }

  getIndent() {
    let depthCount = -1;
    return !this._resource.ParentID ? 0 : this.getResourceDepth(this._resource, depthCount);
  }

  getResourceDepth(resource, depthCount) {
    depthCount++;
    let parentResource = this._resourceList.Items.find(item => item.ID === resource.ParentID);
    return resource.ParentID ? this.getResourceDepth(parentResource, depthCount) : depthCount * 10;
  }

  // FOR HUMOR

  // getIndent() {
  //   console.log('the RESOURCE', this._resource);
  //   if (this._resource.ParentID === null) {
  //     return 0;
  //   }
  //   return this.getResourceDepth(this._resource)
  // }

  // getResourceDepth(resource) {
  //   if (resource.ParentID) {
  //     this._resourceDepth++;
  //     this.getResourceDepth(resource.ParentID);
  //   }
  //   return this._resourceDepth * 10;
  // }

  async toggleNestedResources() {
    const options = { filters: { 'ParentID': this._resource.ID } };
    const categoryResponse = await this.ocCategoryService.List(this._resourceID, options).toPromise();
    let index = this._resourceList.Items.indexOf(this._resource);
    if (!this._isResourceExpanded) {
      categoryResponse.Items.forEach(item => this._resourceList.Items.splice(++index, 0, item));
      this._isResourceExpanded = true;
    } else {
      const collapseBreakPointObject = this._resourceList.Items.find((item, i) => i > index && item.ParentID === this._resourceList.Items[index].ParentID) || 0;
      const identifyCollapseBreakPointIndex = this._resourceList.Items.findIndex(item => item === collapseBreakPointObject);
      const numberToCollapse = identifyCollapseBreakPointIndex !== -1 ? identifyCollapseBreakPointIndex - index - 1 : this._resourceList.Items.length - 1;
      this._resourceList.Items.splice(++index, numberToCollapse);
      this._isResourceExpanded = false;
    }
  }
}
