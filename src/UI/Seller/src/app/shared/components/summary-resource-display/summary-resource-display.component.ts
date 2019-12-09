import { Component, Input } from '@angular/core';
import { singular } from 'pluralize';
import {
  PRODUCT_IMAGE_PATH_STRATEGY,
  getProductMainImageUrlOrPlaceholder,
  PLACEHOLDER_URL,
} from '@app-seller/shared/services/product/product-image.helper';
import { SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY } from '@app-seller/shared/services/configuration/table-display';

@Component({
  selector: 'summary-resource-display-component',
  templateUrl: './summary-resource-display.component.html',
  styleUrls: ['./summary-resource-display.component.scss'],
})
export class SummaryResourceDisplay {
  _primaryHeader = '';
  _secondaryHeader = '';
  _imgPath = '';
  _shouldShowImage = false;
  _isNewPlaceHolder = false;

  @Input()
  resourceType: any;
  @Input()
  set isNewPlaceHolder(value: boolean) {
    this._isNewPlaceHolder = value;
    this.setDisplayValuesForPlaceholder();
  }
  @Input()
  set resource(value: any) {
    this.setDisplayValuesForResource(value);
  }

  setDisplayValuesForResource(resource: any) {
    this._primaryHeader = this.getValueOnExistingResource(resource, 'toPrimaryHeader');
    this._secondaryHeader = this.getValueOnExistingResource(resource, 'toSecondaryHeader');
    this._shouldShowImage = !!SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY[this.resourceType]['toImage'];
    this._imgPath = this.getValueOnExistingResource(resource, 'toImage') || PLACEHOLDER_URL;
  }

  getValueOnExistingResource(value: any, valueType: string) {
    const pathToValue = SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY[this.resourceType][valueType];
    const piecesOfPath = pathToValue.split('.');
    if (pathToValue) {
      if (pathToValue === PRODUCT_IMAGE_PATH_STRATEGY) {
        return getProductMainImageUrlOrPlaceholder(value);
      } else {
        let currentObject = value;
        piecesOfPath.forEach((piece) => {
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
