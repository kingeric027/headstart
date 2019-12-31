import { Component, Input, ViewChild, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { faFilter, faHome } from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { Router, ActivatedRoute } from '@angular/router';
import { takeWhile } from 'rxjs/operators';
import { singular } from 'pluralize';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
import { ListResource } from '@app-seller/shared/services/resource-crud/resource-crud.types';

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
  selectedParentResourceName = 'Fetching Data';
  alive = true;
  psHeight: number = 450;

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
    // Need to use a timeout here to ensure DOM is totally loaded before getting element heights
    setTimeout(() => {
      this.getPsHeight();
    }, 500);
  }

  private setParentResourceSubscription() {
    this.parentService.resourceSubject.pipe(takeWhile(() => this.alive)).subscribe(resourceList => {
      this.parentResourceList = resourceList;
      this.changeDetectorRef.detectChanges();
    });
  }
  //TODO: Move this into a service, it's being used in two different components.
  private getPsHeight() {
    let divsToCaluclate: any = Array.from(document.getElementsByClassName('calculate')),
      totalHeight: number = 0;
    divsToCaluclate.forEach(div => {
      totalHeight += div.offsetHeight;
    });
    this.psHeight = window.innerHeight - totalHeight;
  }

  setParentResourceSelectionSubscription() {
    this.activatedRoute.params.pipe(takeWhile(() => this.alive)).subscribe(async params => {
      if (this.parentService.getParentResourceID() !== REDIRECT_TO_FIRST_PARENT) {
        const parentIDParamName = `${singular(this.parentService.primaryResourceLevel)}ID`;
        const resourceID = params[parentIDParamName];
        if (params && resourceID) {
          const resource = await this.parentService.findOrGetResourceByID(resourceID);
          this.selectedParentResourceName = resource.Name;
        }
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
