import { OnInit, OnDestroy, ChangeDetectorRef, AfterContentInit, NgZone, createPlatform } from '@angular/core';
import { Meta } from '@ordercloud/angular-sdk';
import { takeWhile } from 'rxjs/operators';
import {
  ListResource,
  Options,
  ResourceCrudService,
  FilterDictionary,
} from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { singular } from 'pluralize';
import { resource } from 'selenium-webdriver/http';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';

export abstract class ResourceCrudComponent<ResourceType> implements OnInit, OnDestroy {
  alive = true;
  resourceList: ListResource<ResourceType> = { Meta: {}, Items: [] };
  resourceOptions: Options = {};

  // empty string if no resource is selected
  selectedResourceID = '';
  updatedResource = {};
  resourceInSelection = {};

  resourceForm: FormGroup;
  isValidResource: boolean;

  // form setting defined in component implementing this component
  createForm: (resource: any) => FormGroup;

  ocService: ResourceCrudService<ResourceType>;
  filterForm: FormGroup;
  filterConfig: any = {};
  router: Router;
  isCreatingNew: boolean;

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    ocService: any,
    router: Router,
    private activatedRoute: ActivatedRoute,
    private ngZone: NgZone,
    createForm?: (resource: any) => FormGroup
  ) {
    this.ocService = ocService;
    this.router = router;
    this.createForm = createForm;
  }

  public navigate(url: string, options: any): void {
    /* 
    * Had a bug where clicking on a resource on the second page of resources was triggering an error
    * navigation trigger outside of Angular zone. Might be caused by inheritance or using 
    * changeDetector.detectChange, but couldn't resolve any other way
    * Please remove the need for this if you can
    * https://github.com/angular/angular/issues/25837
    */
    if (Object.keys(options)) {
      this.ngZone.run(() => this.router.navigate([url], options)).then();
    } else {
      this.ngZone.run(() => this.router.navigate([url])).then();
    }
  }

  ngOnInit() {
    this.setFilterForm();
    this.subscribeToResources();
    this.subscribeToOptions();
    this.subscribeToResourceSelection();
    this.setForm(this.updatedResource);
  }

  subscribeToResources() {
    this.ocService.resourceSubject.pipe(takeWhile(() => this.alive)).subscribe((resourceList) => {
      this.resourceList = resourceList;
      this.changeDetectorRef.detectChanges();
    });
  }

  subscribeToOptions() {
    this.ocService.optionsSubject.pipe(takeWhile(() => this.alive)).subscribe((options) => {
      this.resourceOptions = options;
      this.setFilterForm();
      this.changeDetectorRef.detectChanges();
    });
  }

  subscribeToResourceSelection() {
    this.activatedRoute.params.subscribe((params) => {
      if (this.ocService.getParentResourceID() !== REDIRECT_TO_FIRST_PARENT) {
        this.setIsCreatingNew();
        const resourceIDSelected =
          params[`${singular(this.ocService.secondaryResourceLevel || this.ocService.primaryResourceLevel)}ID`];
        if (resourceIDSelected) {
          this.setResourceSelection(resourceIDSelected);
        }
        if (this.isCreatingNew) {
          this.setResoureObjectsForCreatingNew();
        }
      }
    });
  }

  setForm(resource: any) {
    if (this.createForm) {
      this.resourceForm = this.createForm(resource);
      this.changeDetectorRef.detectChanges();
    }
  }

  resetForm(resource: any) {
    if (this.createForm) {
      this.resourceForm.reset(this.createForm(resource));
      this.changeDetectorRef.detectChanges();
    }
  }

  private setIsCreatingNew() {
    const routeUrl = this.router.routerState.snapshot.url;
    const endUrl = routeUrl.slice(routeUrl.length - 4, routeUrl.length);
    this.isCreatingNew = endUrl === '/new';
  }

  handleScrollEnd() {
    if (this.resourceList.Meta.TotalPages > this.resourceList.Meta.Page) {
      this.ocService.getNextPage();
    }
  }

  searchResources(searchStr: string) {
    this.ocService.searchBy(searchStr);
  }

  async setResourceSelection(resourceID: string) {
    this.selectedResourceID = resourceID || '';
    const resource = await this.ocService.findOrGetResourceByID(resourceID);
    this.resourceInSelection = this.copyResource(resource);
    this.setUpdatedResourceAndResourceForm(resource);
  }

  setResoureObjectsForCreatingNew() {
    this.resourceInSelection = this.ocService.emptyResource;
    this.setUpdatedResourceAndResourceForm(this.ocService.emptyResource);
  }

  selectResource(resource: any) {
    const [newURL, queryParams] = this.ocService.constructNewRouteInformation(resource.ID || '');
    this.navigate(newURL, { queryParams });
  }

  updateResource(resourceUpdate: any) {
    // copying a resetting this.updated resource ensures that the copy and base object
    // reference is broken
    // not the prettiest function, feel free to improve
    const piecesOfField = resourceUpdate.field.split('.');
    const depthOfField = piecesOfField.length;
    const updatedResourceCopy = this.copyResource(this.updatedResource);
    switch (depthOfField) {
      case 4:
        updatedResourceCopy[piecesOfField[0]][piecesOfField[1]][piecesOfField[2]][piecesOfField[3]] =
          resourceUpdate.value;
        break;
      case 3:
        updatedResourceCopy[piecesOfField[0]][piecesOfField[1]][piecesOfField[2]] = resourceUpdate.value;
        break;
      case 2:
        updatedResourceCopy[piecesOfField[0]][piecesOfField[1]] = resourceUpdate.value;
        break;
      default:
        updatedResourceCopy[piecesOfField[0]] = resourceUpdate.value;
        break;
    }
    this.updatedResource = updatedResourceCopy;
    this.isValidResource = this.resourceForm.status === 'VALID';
    console.log(this.resourceForm);
    this.changeDetectorRef.detectChanges();
  }

  handleUpdateResource(event: any, field: string) {
    const resourceUpdate = {
      field: field,
      value: event.target.value,
    };
    this.updateResource(resourceUpdate);
  }

  copyResource(resource: any) {
    return JSON.parse(JSON.stringify(resource));
  }

  async saveUpdates() {
    if (this.isCreatingNew) {
      this.createNewResource();
    } else {
      this.updateExitingResource();
    }
  }

  async deleteResource() {
    await this.ocService.deleteResource(this.selectedResourceID);
    this.selectResource({});
  }

  discardChanges() {
    this.setUpdatedResourceAndResourceForm(this.resourceInSelection);
  }

  async updateExitingResource() {
    const updatedResource = await this.ocService.updateResource(this.updatedResource);
    this.resourceInSelection = this.copyResource(updatedResource);
    this.setUpdatedResourceAndResourceForm(updatedResource);
  }

  setUpdatedResourceAndResourceForm(updatedResource: any) {
    this.updatedResource = this.copyResource(updatedResource);
    this.setForm(updatedResource);
    this.changeDetectorRef.detectChanges();
  }

  async createNewResource() {
    const newResource = await this.ocService.createNewResource(this.updatedResource);
    this.selectResource(newResource);
  }

  applyFilters() {
    this.ocService.addFilters(this.removeFieldsWithNoValue(this.filterForm.value));
  }

  removeFieldsWithNoValue(formValues: FilterDictionary) {
    const values = { ...formValues };
    Object.entries(values).forEach(([key, value]) => {
      if (!value) {
        delete values[key];
      }
    });
    return values;
  }

  setFilterForm() {
    const formGroup = {};
    if (this.filterConfig && this.filterConfig.Filters) {
      this.filterConfig.Filters.forEach((filter) => {
        const value = (this.resourceOptions.filters && this.resourceOptions.filters[filter.Path]) || '';
        formGroup[filter.Path] = new FormControl(value);
      });
      this.filterForm = new FormGroup(formGroup);
    }
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
