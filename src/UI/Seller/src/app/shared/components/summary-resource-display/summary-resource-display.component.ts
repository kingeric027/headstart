import { Component, Input } from '@angular/core';

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
  set resource(value: any) {
    this._primaryHeader = this.getValueOnResource(value, 'toPrimaryHeader');
    this._secondaryHeader = this.getValueOnResource(value, 'toSecondaryHeader');
    this._imgPath = this.getValueOnResource(value, 'toImage');
  }

  getValueOnResource(value: any, valueType: string) {
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
}
