import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { singular } from 'pluralize';
import {
  PRODUCT_IMAGE_PATH_STRATEGY,
  getProductMainImageUrlOrPlaceholder,
  PLACEHOLDER_URL,
} from '@app-seller/shared/services/product/product-image.helper';
import {
  faChevronDown,
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

  setDisplayValuesForResource(resource: any) {
    this._primaryHeader = this.getValueOnExistingResource(resource, 'toPrimaryHeader');
    this._secondaryHeader = this.getValueOnExistingResource(resource, 'toSecondaryHeader');
    this._shouldShowImage = !!SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY[this.resourceType]['toImage'];
    this._imgPath = this.getValueOnExistingResource(resource, 'toImage') || PLACEHOLDER_URL;
    this._isExpandable = !!SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY[this.resourceType]['toExpandable'];
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
}
