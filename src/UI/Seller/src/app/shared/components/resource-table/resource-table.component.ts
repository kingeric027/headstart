import { Component, Input, Output, ViewChild, OnInit, ChangeDetectorRef, OnDestroy, NgZone } from '@angular/core';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { EventEmitter } from '@angular/core';
import { faFilter, faChevronLeft, faHome } from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { takeWhile, filter } from 'rxjs/operators';
import { singular } from 'pluralize';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
import { FormGroup } from '@angular/forms';
import { ListResource, Options, RequestStatus } from '@app-seller/shared/services/resource-crud/resource-crud.types';
import { getScreenSizeBreakPoint, getPsHeight } from '@app-seller/shared/services/dom.helper';

interface BreadCrumb {
  displayText: string;
  route: string;
}

@Component({
  selector: 'resource-table-component',
  templateUrl: './resource-table.component.html',
  styleUrls: ['./resource-table.component.scss'],
})
export class ResourceTableComponent implements OnInit, OnDestroy {
  @ViewChild('popover', { static: false })
  public popover: NgbPopover;
  faFilter = faFilter;
  faHome = faHome;
  faChevronLeft = faChevronLeft;
  searchTerm = '';
  _resourceOptions: Options;
  _resourceInSelection: any;
  _updatedResource: any;
  _selectedResourceID: string;
  _currentResourceNamePlural: string;
  _currentResourceNameSingular: string;
  _ocService: ResourceCrudService<any>;
  areChanges: boolean;
  parentResources: ListResource<any>;
  requestStatus: RequestStatus;
  selectedParentResourceName = 'Fetching Data';
  selectedParentResourceID = '';
  breadCrumbs: BreadCrumb[] = [];
  isCreatingNew = false;
  isMyResource = false;
  alive = true;
  screenSize;
  myResourceHeight: number = 450;
  tableHeight: number = 450;
  editResourceHeight: number = 450;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private changeDetectorRef: ChangeDetectorRef,
    ngZone: NgZone
  ) {}

  @Input()
  resourceList: ListResource<any> = { Meta: {}, Items: [] };
  @Input()
  set ocService(service: ResourceCrudService<any>) {
    this._ocService = service;
    this._currentResourceNamePlural = service.secondaryResourceLevel || service.primaryResourceLevel;
    this._currentResourceNameSingular = singular(this._currentResourceNamePlural);
  }
  @Input()
  parentResourceService?: ResourceCrudService<any>;
  @Output()
  searched: EventEmitter<any> = new EventEmitter();
  @Output()
  hitScrollEnd: EventEmitter<any> = new EventEmitter();
  @Output()
  changesSaved: EventEmitter<any> = new EventEmitter();
  @Output()
  resourceDelete: EventEmitter<any> = new EventEmitter();
  @Output()
  changesDiscarded: EventEmitter<any> = new EventEmitter();
  @Output()
  resourceSelected: EventEmitter<any> = new EventEmitter();
  @Output()
  applyFilters: EventEmitter<any> = new EventEmitter();
  @Input()
  set updatedResource(value: any) {
    this._updatedResource = value;
    this.checkForChanges();
  }
  @Input()
  set resourceInSelection(value: any) {
    this._resourceInSelection = value;
    this.checkForChanges();
  }
  @Input()
  set resourceOptions(value: Options) {
    this._resourceOptions = value;
    this.searchTerm = (value && value.search) || '';
  }
  @Input()
  selectedResourceID: string;
  @Input()
  resourceForm: FormGroup;

  async ngOnInit() {
    this.determineViewingContext();
    this.initializeSubscriptions();
    this.screenSize = getScreenSizeBreakPoint();
    this.getPerfectScrollHeights();
  }

  getPerfectScrollHeights() {
    // Need to use a timeout here to ensure DOM is totally loaded before getting element heights
    setTimeout(() => {
      this.myResourceHeight = getPsHeight('');
      this.tableHeight = getPsHeight('additional-item-table');
      this.editResourceHeight = getPsHeight('additional-item-edit-resource');
    }, 500);
  }
  determineViewingContext() {
    this.isMyResource = this.router.url.startsWith('/my-');
  }

  private async initializeSubscriptions() {
    await this.redirectToFirstParentIfNeeded();
    this.setUrlSubscription();
    this.setParentResourceSelectionSubscription();
    this.setListRequestStatusSubscription();
    this._ocService.listResources();
  }

  private async redirectToFirstParentIfNeeded() {
    if (this.parentResourceService) {
      if (this.parentResourceService.getParentResourceID() === REDIRECT_TO_FIRST_PARENT) {
        await this.parentResourceService.listResources();
        this._ocService.selectParentResource(this.parentResourceService.resourceSubject.value.Items[0]);
      }
    }
  }

  private setUrlSubscription() {
    this.router.events
      .pipe(takeWhile(() => this.alive))
      // only need to set the breadcrumbs on nav end events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.setBreadCrumbs();
      });
    this.activatedRoute.params.pipe(takeWhile(() => this.alive)).subscribe(() => {
      this.setBreadCrumbs();
      this.checkIfCreatingNew();
    });
  }

  private setParentResourceSelectionSubscription() {
    this.activatedRoute.params
      .pipe(takeWhile(() => this.parentResourceService && this.alive))
      .subscribe(async params => {
        await this.redirectToFirstParentIfNeeded();
        const parentIDParamName = `${singular(this._ocService.primaryResourceLevel)}ID`;
        const parentResourceID = params[parentIDParamName];
        this.selectedParentResourceID = parentResourceID;
        if (params && parentResourceID) {
          const parentResource = await this.parentResourceService.findOrGetResourceByID(parentResourceID);
          if (parentResource) this.selectedParentResourceName = parentResource.Name;
        }
      });
  }

  private setListRequestStatusSubscription() {
    this._ocService.resourceRequestStatus.pipe(takeWhile(() => this.alive)).subscribe(requestStatus => {
      this.requestStatus = requestStatus;
      this.changeDetectorRef.detectChanges();
    });
  }

  private checkIfCreatingNew() {
    const routeUrl = this.router.routerState.snapshot.url;
    const endUrl = routeUrl.slice(routeUrl.length - 4, routeUrl.length);
    this.isCreatingNew = endUrl === '/new';
  }

  private setBreadCrumbs() {
    // basically we are just taking off the portion of the url after the selected route piece
    // in the future breadcrumb logic might need to be more complicated than this
    const urlPieces = this.router.url
      .split('/')
      .filter(p => p)
      .map(p => {
        if (p.includes('?')) {
          return p.slice(0, p.indexOf('?'));
        } else {
          return p;
        }
      });
    this.breadCrumbs = urlPieces.map((piece, index) => {
      const route = `/${urlPieces.slice(0, index + 1).join('/')}`;
      return {
        displayText: piece,
        route,
      };
    });
    this.changeDetectorRef.detectChanges();
  }

  searchedResources(event) {
    this.searched.emit(event);
  }

  handleScrollEnd() {
    this.hitScrollEnd.emit(null);
  }

  handleSave() {
    this.changesSaved.emit(null);
  }

  handleDelete() {
    this.resourceDelete.emit(null);
  }

  handleDiscardChanges() {
    this.changesDiscarded.emit(null);
  }

  handleSelectResource(resource: any) {
    const [newURL, queryParams] = this._ocService.constructNewRouteInformation(resource.ID || '');
    this.router.navigate([newURL], { queryParams });
    this.resourceSelected.emit(resource);
  }

  openPopover() {
    this.popover.open();
  }

  closePopover() {
    this.popover.close();
  }

  handleApplyFilters() {
    this.closePopover();
    this.applyFilters.emit(null);
  }

  clearAllFilters() {
    this._ocService.clearAllFilters();
  }

  checkForChanges() {
    this.areChanges = JSON.stringify(this._updatedResource) !== JSON.stringify(this._resourceInSelection);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
