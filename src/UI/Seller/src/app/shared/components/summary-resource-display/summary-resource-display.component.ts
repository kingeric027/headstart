import { Component, Input } from '@angular/core';
import { singular } from 'pluralize';

interface ResourceInfoPath {
  toPrimaryHeader: string;
  toSecondaryHeader: string;
  toImage: string;
}

interface ResourceInfoPathsDictionary {
  [resourceType: string]: ResourceInfoPath;
}

@Component({
  selector: 'summary-resource-display-component',
  templateUrl: './summary-resource-display.component.html',
  styleUrls: ['./summary-resource-display.component.scss'],
})
export class SummaryResourceDisplay {
  _primaryHeader = '';
  _secondaryHeader = '';
  _imgPath = '';
  _isNewPlaceHolder = false;

  resourceTypeMap: ResourceInfoPathsDictionary = {
    suppliers: {
      toPrimaryHeader: 'Name',
      toSecondaryHeader: '',
      toImage: '',
    },
    users: {
      toPrimaryHeader: 'Name',
      toSecondaryHeader: 'ID',
      toImage: '',
    },
    products: {
      toPrimaryHeader: 'Name',
      toSecondaryHeader: 'ID',
      toImage: '',
    },
    promotions: {
      toPrimaryHeader: 'Name',
      toSecondaryHeader: 'Code',
      toImage: '',
    },
    buyers: {
      toPrimaryHeader: 'Name',
      toSecondaryHeader: 'ID',
      toImage: '',
    },
    locations: {
      toPrimaryHeader: 'Name',
      toSecondaryHeader: 'ID',
      toImage: '',
    },
  };

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
    this._imgPath = this.getValueOnExistingResource(resource, 'toImage');
  }

  getValueOnExistingResource(value: any, valueType: string) {
    const pathToValue = this.resourceTypeMap[this.resourceType][valueType];
    const piecesOfPath = pathToValue.split('.');
    if (pathToValue) {
      let currentObject = value;
      piecesOfPath.forEach((piece) => {
        currentObject = currentObject && currentObject[piece];
      });
      return currentObject;
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
