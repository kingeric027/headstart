import { Component, Input, ViewChild, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ListResource, ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { faFilter, faHome } from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { Router, ActivatedRoute } from '@angular/router';
import { takeWhile } from 'rxjs/operators';
import { singular } from 'pluralize';

@Component({
  selector: 'resource-select-dropdown-component',
  templateUrl: './resource-select-dropdown.component.html',
  styleUrls: ['./resource-select-dropdown.component.scss'],
})
export class ResourceSelectDropdown implements OnInit, OnDestroy {
  @ViewChild('popover', { static: false })
  public popover: NgbPopover;
  faFilter = faFilter;
  faHome = faHome;
  searchTerm = '';
  selectedParentResourceName = 'Parent Resource Name';
  alive = true;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  @Input()
  ocService: ResourceCrudService<any>;
  @Input()
  parentService: ResourceCrudService<any>;
  parentResourceList: ListResource<any> = { Meta: {}, Items: [] };

  ngOnInit() {
    this.setParentResourceSubscription();
    this.setParentResourceSelectionSubscription();
  }

  private setParentResourceSubscription() {
    this.parentService.resourceSubject.subscribe((resourceList) => {
      this.parentResourceList = resourceList;
      this.changeDetectorRef.detectChanges();
    });
  }

  setParentResourceSelectionSubscription() {
    this.activatedRoute.params.pipe(takeWhile(() => this.alive)).subscribe(async (params) => {
      const parentIDParamName = `${singular(this.parentService.primaryResourceLevel)}ID`;
      const resourceID = params[parentIDParamName];
      if (params && resourceID) {
        const resource = await this.parentService.findOrGetResourceByID(resourceID);
        this.selectedParentResourceName = resource.Name;
      }
    });
  }

  selectParentResource(resource: any) {
    this.ocService.selectParentResource(resource);

    // reset the search form when selecting resource
    this.parentService.listResources();
    this.searchTerm = '';
  }

  searchedResources(searchText: any) {
    this.parentService.listResources(1, searchText);
    this.searchTerm = searchText;
  }

  handleScrollEnd() {
    const totalPages = this.parentResourceList.Meta.TotalPages;
    const nextPageNumber = this.parentResourceList.Meta.Page + 1;
    if (totalPages >= nextPageNumber) this.parentService.listResources(nextPageNumber, this.searchTerm);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
