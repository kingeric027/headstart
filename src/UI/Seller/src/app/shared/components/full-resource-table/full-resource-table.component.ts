import { Component, Input, Output, EventEmitter } from '@angular/core';
import {
  PRODUCT_IMAGE_PATH_STRATEGY,
  getProductMainImageUrlOrPlaceholder,
  PLACEHOLDER_URL,
} from '@app-seller/shared/services/product/product-image.helper';
import {
  FULL_TABLE_RESOURCE_DICTIONARY,
  ResourceCell,
  ResourceColumnConfiguration,
  ResourceRow,
  ResourceConfiguration,
} from '@app-seller/shared/services/configuration/table-display';

@Component({
  selector: 'full-resource-table-component',
  templateUrl: './full-resource-table.component.html',
  styleUrls: ['./full-resource-table.component.scss'],
})
export class FullResourceTableComponent {
  headers = [];
  rows = [];
  numberOfColumns = 1;
  _resourceList = { Meta: {}, Items: [] };

  @Input()
  resourceType: any;
  @Input()
  set resourceList(value: any) {
    this._resourceList = value;
    this.setDisplayValuesForResource(value.Items);
  }
  @Output()
  resourceSelected = new EventEmitter();

  setDisplayValuesForResource(resources: any[] = []) {
    this.headers = this.getHeaders(resources);
    this.rows = this.getRows(resources);
    this.numberOfColumns = this.getNumberOfColumns(this.resourceType);
  }

  getHeaders(resources: any[]): string[] {
    return FULL_TABLE_RESOURCE_DICTIONARY[this.resourceType].fields.map((r) => r.header);
  }

  getRows(resources: any[]): ResourceRow[] {
    return resources.map((resource) => {
      return this.createResourceRow(resource);
    });
  }

  getNumberOfColumns(resourceType: string): number {
    return FULL_TABLE_RESOURCE_DICTIONARY[resourceType].fields.length;
  }

  createResourceRow(resource: any): ResourceRow {
    const resourceConfiguration = FULL_TABLE_RESOURCE_DICTIONARY[this.resourceType];
    const fields = resourceConfiguration.fields;
    const resourceCells = fields.map((fieldConfiguration) => {
      return {
        type: fieldConfiguration.type,
        value: this.getValueOnExistingResource(resource, fieldConfiguration.path),
      };
    });
    return {
      resource,
      cells: resourceCells,
      imgPath: resourceConfiguration.imgPath ? this.getImage(resource, resourceConfiguration) : '',
    };
  }

  getImage(resource: any, resourceConfiguration: ResourceConfiguration): string {
    let imgUrl = '';
    if (resourceConfiguration.imgPath === PRODUCT_IMAGE_PATH_STRATEGY) {
      imgUrl = getProductMainImageUrlOrPlaceholder(resource);
    } else {
      imgUrl = this.getValueOnExistingResource(resource, FULL_TABLE_RESOURCE_DICTIONARY[this.resourceType].imgPath);
    }
    return imgUrl || PLACEHOLDER_URL;
  }

  selectResource(value: any) {
    this.resourceSelected.emit(value);
  }

  getValueOnExistingResource(value: any, path: string) {
    const piecesOfPath = path.split('.');
    if (path) {
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
