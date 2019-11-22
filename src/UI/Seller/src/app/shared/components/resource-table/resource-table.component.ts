import { Component, Input, Output, ViewChild, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import {
  ListResource,
  ResourceCrudService,
  Options,
} from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { EventEmitter } from '@angular/core';
import { faFilter, faChevronLeft } from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { Router, ActivatedRoute } from '@angular/router';
import { takeWhile } from 'rxjs/operators';

interface BreadCrumb {
  text: string;
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
  faChevronLeft = faChevronLeft;
  searchTerm = '';
  _resourceOptions: Options;
  _resourceInSelection: any;
  _updatedResource: any;
  areChanges: boolean;
  parentResources: ListResource<any>;
  breadCrumbs: string[] = [];
  alive = true;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  @Input()
  resourceList: ListResource<any> = { Meta: {}, Items: [] };
  @Input()
  selectedResourceID: string;
  @Input()
  ocService: ResourceCrudService<any>;
  @Input()
  parentResourceService?: ResourceCrudService<any>;
  @Input()
  resourceName: string;
  @Output()
  searched: EventEmitter<any> = new EventEmitter();
  @Output()
  hitScrollEnd: EventEmitter<any> = new EventEmitter();
  @Output()
  changesSaved: EventEmitter<any> = new EventEmitter();
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

  ngOnInit() {
    this.setParentResourceSubscription();
    this.setUrlSubscription();
  }

  private setParentResourceSubscription() {
    if (this.parentResourceService) {
      this.parentResourceService.resourceSubject.subscribe((parentResources) => {
        this.parentResources = parentResources;
      });
    }
  }

  private setUrlSubscription() {
    this.router.events.pipe(takeWhile(() => this.alive)).subscribe(() => {
      this.setBreadCrumbs();
    });
    this.activatedRoute.params.pipe(takeWhile(() => this.alive)).subscribe(() => {
      this.setBreadCrumbs();
    });
  }

  private setBreadCrumbs() {
    this.breadCrumbs = this.router.url.split('/').filter((p) => p);
    this.changeDetectorRef.detectChanges();
  }

  selectParentResource(resource: any) {
    this.ocService.selectParentResource(resource);
  }

  searchedResources(event) {
    this.searched.emit(event);
  }

  handleScrollEnd() {
    this.hitScrollEnd.emit(null);
  }

  handleSaveUpdates() {
    this.changesSaved.emit(null);
  }

  handleSelectResource(resource: any) {
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
    this.ocService.clearAllFilters();
  }

  checkForChanges() {
    this.areChanges = JSON.stringify(this._updatedResource) !== JSON.stringify(this._resourceInSelection);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
