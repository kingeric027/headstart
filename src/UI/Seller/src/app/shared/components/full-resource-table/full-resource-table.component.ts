import { Component, Input, Output, EventEmitter } from '@angular/core';
import {
  FULL_TABLE_RESOURCE_DICTIONARY,
  ResourceRow,
  ResourceConfiguration,
} from '@app-seller/shared/services/configuration/table-display';
import { RequestStatus } from '@app-seller/shared/services/resource-crud/resource-crud.types';
import {
  PRODUCT_IMAGE_PATH_STRATEGY,
  getProductMainImageUrlOrPlaceholder,
  PLACEHOLDER_URL,
} from '@app-seller/products/product-image.helper';
import { faCopy, faSort } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'full-resource-table-component',
  templateUrl: './full-resource-table.component.html',
  styleUrls: ['./full-resource-table.component.scss'],
})
export class FullResourceTableComponent {
  headers = [];
  rows = [];
  numberOfColumns = 1;
  faCopy = faCopy;
  faSort = faSort;
  sortAsc: boolean;
  activeSort: string;
  objectPreviewText: string;
  _resourceList = { Meta: {}, Items: [] };

  @Input()
  resourceType: any;
  @Input()
  requestStatus: RequestStatus;
  @Input()
  set resourceList(value: any) {
    this._resourceList = value;
    this.setDisplayValuesForResource(value.Items);
  }
  @Output()
  resourceSelected = new EventEmitter();

  constructor(
    private toastrService: ToastrService,
    private router: Router
  ) {}

  setDisplayValuesForResource(resources: any[] = []) {
    this.headers = this.getHeaders(resources);
    this.rows = this.getRows(resources);
    this.numberOfColumns = this.getNumberOfColumns(this.resourceType);
  }

  getHeaders(resources: any[]): object[] {
    return FULL_TABLE_RESOURCE_DICTIONARY[this.resourceType].fields.map(r => r);
  }

  getRows(resources: any[]): ResourceRow[] {
    return resources.map(resource => {
      return this.createResourceRow(resource);
    });
  }

  getNumberOfColumns(resourceType: string): number {
    return FULL_TABLE_RESOURCE_DICTIONARY[resourceType].fields.length;
  }

  createResourceRow(resource: any): ResourceRow {
    const resourceConfiguration = FULL_TABLE_RESOURCE_DICTIONARY[this.resourceType];
    const fields = resourceConfiguration.fields;
    const resourceCells = fields.map(fieldConfiguration => {
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

  copyObject(resource: any) {
    this.toastrService.success(null, 'Copied to clipboard!', {
      disableTimeOut: false,
      closeButton: true,
      tapToDismiss: true,
    });
    let copy = document.createElement("textarea");
    document.body.appendChild(copy);
    copy.value = JSON.stringify(resource);
    copy.select();
    document.execCommand("copy");
    document.body.removeChild(copy);
  }

  previewObject(resource: any) {
    this.objectPreviewText = JSON.stringify(resource);
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
      piecesOfPath.forEach(piece => {
        currentObject = currentObject && currentObject[piece];
      });
      return currentObject;
    } else {
      return '';
    }
  }

  handleSort(header: string): void {
    if (this.activeSort === header) {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortAsc = true;
    }
    this.activeSort = header; //Track current sort to determine if next sort should be ASC or DESC.
    let routeUrl = this.router.routerState.snapshot.url;
    if (routeUrl.includes('?sortBy=')) {
      let splitUrl = routeUrl.split('/');
      let urlWithoutParams = splitUrl[splitUrl.length - 1].split('?');
      splitUrl[splitUrl.length - 1] = urlWithoutParams[0];
      routeUrl = splitUrl.join('/');
    }
    if (this.sortAsc) {
      this.router.navigate([`${routeUrl}`], { queryParams: { sortBy: header } });
    } else if (!this.sortAsc) {
      this.router.navigate([`${routeUrl}`], { queryParams: { sortBy: '!' + header } });
    }
  }
}
