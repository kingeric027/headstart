import { AfterViewChecked, ChangeDetectorRef, Component, EventEmitter, Input, NgZone, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
import { getPsHeight, getScreenSizeBreakPoint } from '@app-seller/shared/services/dom.helper';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { FilterDictionary, ListResource, Options, RequestStatus } from '@app-seller/shared/services/resource-crud/resource-crud.types';
import { faCalendar, faChevronLeft, faFilter, faHome, faTimes } from '@fortawesome/free-solid-svg-icons';
import { NgbDateStruct, NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { singular } from 'pluralize';
import { filter, takeWhile } from 'rxjs/operators';

interface BreadCrumb {
  displayText: string;
  route: string;
}

@Component({
  selector: 'resource-table-component',
  templateUrl: './resource-table.component.html',
  styleUrls: ['./resource-table.component.scss'],
  host: {
    '(window:resize)': 'ngAfterViewChecked()',
  },
})
export class ResourceTableComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('popover', { static: false })
  public popover: NgbPopover;
  faFilter = faFilter;
  faTimes = faTimes;
  faHome = faHome;
  faChevronLeft = faChevronLeft;
  faCalendar = faCalendar;
  searchTerm = '';
  resourceOptions: Options;
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
  myResourceHeight = 450;
  tableHeight = 450;
  editResourceHeight = 450;
  activeFilterCount = 0;
  filterForm: FormGroup;
  fromDate: string;
  toDate: string;
  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private changeDetectorRef: ChangeDetectorRef,
    ngZone: NgZone
  ) {
  }

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
  selectedResourceID: string;
  @Input()
  filterConfig: any;
  @Input()
  resourceForm: FormGroup;
  @Input()
  shouldShowCreateNew = true;
  @Input()
  shouldShowResourceActions = true;
  @Input()
  dataIsSaving = false;

  async ngOnInit() {
    this.determineViewingContext();
    this.initializeSubscriptions();
    this.setFilterForm();
    this.subscribeToOptions();
    this.screenSize = getScreenSizeBreakPoint();
  }

  ngAfterViewChecked() {
    this.setPsHeights();
    this.changeDetectorRef.detectChanges();
  }

  subscribeToOptions() {
    this._ocService.optionsSubject.pipe(takeWhile(() => this.alive)).subscribe(options => {
      this.resourceOptions = options;
      this.searchTerm = (options && options.search) || '';
      this.activeFilterCount = options.filters ? Object.keys(options.filters).length : 0;
      this.setFilterForm();
      this.changeDetectorRef.detectChanges();
    });
  }

  applyFilters() {
    if (typeof this.filterForm.value['from'] === 'object') {
      const fromDate = this.filterForm.value['from'];
      this.fromDate = this.transformDateForUser(fromDate);
      this.filterForm.value['from'] = this.transformDateForFilter(fromDate);
    } if (typeof this.filterForm.value['to'] === 'object') {
      const toDate = this.filterForm.value['to'];
      this.toDate = this.transformDateForUser(toDate);
      this.filterForm.value['to'] = this.transformDateForFilter(toDate);
    }
    this._ocService.addFilters(this.removeFieldsWithNoValue(this.filterForm.value));
  }
  // date format for NgbDatepicker is different than date format used for filters
  transformDateForUser(date: NgbDateStruct) {
    let month = date.month.toString().length === 1 ? '0' + date.month : date.month;
    let day = date.day.toString().length === 1 ? '0' + date.day : date.day;
    return date.year + '-' + month + '-' + day;
  }

  transformDateForFilter(date: NgbDateStruct) {
    return date.month + '-' + date.day + '-' + date.year;
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

  setPsHeights() {
    this.myResourceHeight = getPsHeight('');
    this.tableHeight = getPsHeight('additional-item-table');
    this.editResourceHeight = getPsHeight('additional-item-edit-resource');
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

  setFilterForm() {
    const formGroup = {};
    if (this.filterConfig && this.filterConfig.Filters) {
      this.filterConfig.Filters.forEach(filter => {
        const value = this.getSelectedFilterValue(filter.Path);
        formGroup[filter.Path] = new FormControl(value);
      });
      this.filterForm = new FormGroup(formGroup);
    }
  }

  getSelectedFilterValue(pathOfFilter: string) {
    return (this.resourceOptions && this.resourceOptions.filters && this.resourceOptions.filters[pathOfFilter]) || '';
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
    this.applyFilters();
  }

  clearAllFilters() {
    this._ocService.clearAllFilters();
    this.toDate = '';
    this.fromDate = '';
  }

  checkForChanges() {
    this.areChanges = JSON.stringify(this._updatedResource) !== JSON.stringify(this._resourceInSelection);
  }

  ngOnDestroy() {
    this.alive = false;
  }

}