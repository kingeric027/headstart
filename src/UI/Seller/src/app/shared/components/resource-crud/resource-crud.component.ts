import { OnInit, OnDestroy, ChangeDetectorRef, NgZone, Output } from '@angular/core';
import { takeWhile } from 'rxjs/operators';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { FormGroup } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { singular } from 'pluralize';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
import { ListPage } from '@app-seller/shared/services/middleware-api/listPage.interface';

export abstract class ResourceCrudComponent<ResourceType> implements OnInit, OnDestroy {
  alive = true;
  resourceList: ListPage<ResourceType> = { Meta: {}, Items: [] };

  // empty string if no resource is selected
  selectedResourceID = '';
  updatedResource = {} as ResourceType;
  resourceInSelection = {} as ResourceType;
  resourceToCreate = {} as any;
  resourceForm: FormGroup;
  isMyResource = false;

  // form setting defined in component implementing this component
  createForm: (resource: any) => FormGroup;

  ocService: ResourceCrudService<ResourceType>;
  filterConfig: any = {};
  router: Router;
  isCreatingNew: boolean;
  dataIsSaving = false;

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

  ngOnInit(): void {
    this.determineViewingContext();
    this.subscribeToResources();
    this.subscribeToResourceSelection();
    this.setForm(this.updatedResource);
  }

  subscribeToResources(): void {
    this.ocService.resourceSubject.pipe(takeWhile(() => this.alive)).subscribe(resourceList => {
      this.resourceList = resourceList;
      this.changeDetectorRef.detectChanges();
    });
  }

  async determineViewingContext(): Promise<void> {
    this.isMyResource = this.router.url.startsWith('/my-');
    if (this.isMyResource) {
      const supplier = await this.ocService.getMyResource();
      this.setResourceSelectionFromResource(supplier);
    }
  }

  subscribeToResourceSelection(): void {
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

  setForm(resource: any): void {
    if (this.createForm) {
      this.resourceForm = this.createForm(resource);
      this.changeDetectorRef.detectChanges();
    }
  }

  resetForm(resource: any): void {
    if (this.createForm) {
      this.resourceForm.reset(this.createForm(resource));
      this.changeDetectorRef.detectChanges();
    }
  }

  handleScrollEnd(): void {
    if (this.resourceList.Meta.TotalPages > this.resourceList.Meta.Page) {
      this.ocService.getNextPage();
    }
  }

  searchResources(searchStr: string): void {
    this.ocService.searchBy(searchStr);
  }

  async setResourceSelectionFromID(resourceID: string): Promise<void> {
    this.selectedResourceID = resourceID || '';
    const resource = await this.ocService.findOrGetResourceByID(resourceID);
    this.resourceInSelection = this.copyResource(resource);
    this.setUpdatedResourceAndResourceForm(resource);
  }

  setResourceSelectionFromResource(resource: any): void {
    this.selectedResourceID = (resource && resource.ID) || '';

    this.resourceInSelection = this.copyResource(resource);
    this.setUpdatedResourceAndResourceForm(resource);
  }

  setResoureObjectsForCreatingNew(): void {
    this.resourceInSelection = this.ocService.emptyResource;
    this.setUpdatedResourceAndResourceForm(this.ocService.emptyResource);
  }

  selectResource(resource: any): void {
    const [newURL, queryParams] = this.ocService.constructNewRouteInformation(resource.ID || '');
    this.navigate(newURL, { queryParams });
  }

  updateResource(resourceUpdate: any): void {
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

  handleUpdateResource(event: any, field: string): void {
    const resourceUpdate = {
      field,
      value: field === 'Active' ? event.target.checked : event.target.value,
    };
    this.updateResource(resourceUpdate);
  }

  copyResource(resource: ResourceType): ResourceType {
    return JSON.parse(JSON.stringify(resource));
  }

  saveUpdates(): void {
    if (this.isCreatingNew) {
      this.createNewResource();
    } else {
      this.updateExistingResource();
    }
  }

  async deleteResource(): Promise<void> {
    await this.ocService.deleteResource(this.selectedResourceID);
    this.selectResource({});
  }

  discardChanges(): void {
    this.setUpdatedResourceAndResourceForm(this.resourceInSelection);
  }

  async updateExistingResource(): Promise<void> {
    // dataIsSaving indicator is used in the resource table to conditionally tell the
    // submit button to disable
    try {
      this.dataIsSaving = true;
      const updatedResource = await this.ocService.updateResource(this.updatedResource);
      this.resourceInSelection = this.copyResource(updatedResource);
      this.setUpdatedResourceAndResourceForm(updatedResource);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  setUpdatedResourceAndResourceForm(updatedResource: any): void {
    this.updatedResource = this.copyResource(updatedResource);
    this.setForm(this.copyResource(updatedResource));
    this.changeDetectorRef.detectChanges();
  }

  async createNewResource(): Promise<void> {
    // dataIsSaving indicator is used in the resource table to conditionally tell the
    // submit button to disable
    if (Object.keys(this.resourceToCreate).length === 0) {
      //Only assign this value if a component inheriting this class hasn't already.
      this.resourceToCreate = this.updatedResource;
    }
    try {
      this.dataIsSaving = true;
      const newResource = await this.ocService.createNewResource(this.resourceToCreate);
      this.selectResource(newResource);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  ngOnDestroy(): void {
    this.alive = false;
  }

  // private setIsCreatingNew(): void {
  //   const routeUrl = this.router.routerState.snapshot.url;
  //   const endUrl = routeUrl.slice(routeUrl.length - 4, routeUrl.length);
  //   this.isCreatingNew = endUrl === '/new';
  // }

  private setIsCreatingNew(): void {
    const routeUrl = this.router.routerState.snapshot.url;
    const splitUrl = routeUrl.split('/');
    const endUrl = splitUrl[splitUrl.length - 1];
    /* Reduce possibility of errors: all IDs with the word new must equal it exactly,
    or begin with the word new and have a question mark following it for query params. */
    this.isCreatingNew = endUrl === 'new' || endUrl.startsWith('new?');
  }
}
