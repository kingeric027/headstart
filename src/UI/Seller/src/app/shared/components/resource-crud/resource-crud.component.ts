import { OnInit, OnDestroy, ChangeDetectorRef, AfterContentInit, NgZone, createPlatform } from '@angular/core';
import { Meta } from '@ordercloud/angular-sdk';
import { takeWhile } from 'rxjs/operators';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { singular } from 'pluralize';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
import { ListResource, Options, FilterDictionary } from '@app-seller/shared/services/resource-crud/resource-crud.types';

export abstract class ResourceCrudComponent<ResourceType> implements OnInit, OnDestroy {
  alive = true;
  resourceList: ListResource<ResourceType> = { Meta: {}, Items: [] };

  // empty string if no resource is selected
  selectedResourceID = '';
  updatedResource = {};
  resourceInSelection = {};

  resourceForm: FormGroup;
  isMyResource = false;

  // form setting defined in component implementing this component
  createForm: (resource: any) => FormGroup;

  ocService: ResourceCrudService<ResourceType>;
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
    this.determineViewingContext();
    this.subscribeToResources();
    this.subscribeToResourceSelection();
    this.setForm(this.updatedResource);
  }

  subscribeToResources() {
    this.ocService.resourceSubject.pipe(takeWhile(() => this.alive)).subscribe(resourceList => {
      this.resourceList = resourceList;
      this.changeDetectorRef.detectChanges();
    });
  }

  async determineViewingContext() {
    this.isMyResource = this.router.url.startsWith('/my-');
    if (this.isMyResource) {
      const supplier = await this.ocService.getMyResource();
      this.setResourceSelectionFromResource(supplier);
    }
  }

  subscribeToResourceSelection() {
    this.activatedRoute.params.subscribe(params => {
      if (this.ocService.getParentResourceID() !== REDIRECT_TO_FIRST_PARENT) {
        this.setIsCreatingNew();
        const resourceIDSelected =
          params[`${singular(this.ocService.secondaryResourceLevel || this.ocService.primaryResourceLevel)}ID`];
        if (resourceIDSelected) {
          this.setResourceSelectionFromID(resourceIDSelected);
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

  async setResourceSelectionFromID(resourceID: string) {
    this.selectedResourceID = resourceID || '';
    const resource = await this.ocService.findOrGetResourceByID(resourceID);
    this.resourceInSelection = this.copyResource(resource);
    this.setUpdatedResourceAndResourceForm(resource);
  }

  async setResourceSelectionFromResource(resource: any) {
    this.selectedResourceID = (resource && resource.ID) || '';

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
    this.changeDetectorRef.detectChanges();
  }

  handleUpdateResource(event: any, field: string) {
    const resourceUpdate = {
      field,
      value: field === "Active" ? event.target.checked : event.target.value,
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
    this.setForm(this.copyResource(updatedResource));
    this.changeDetectorRef.detectChanges();
  }

  async createNewResource() {
    const newResource = await this.ocService.createNewResource(this.updatedResource);
    this.selectResource(newResource);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
