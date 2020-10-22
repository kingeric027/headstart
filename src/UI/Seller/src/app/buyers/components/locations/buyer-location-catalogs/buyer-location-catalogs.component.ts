import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MarketplaceCatalog, MarketplaceCatalogAssignmentRequest } from '@ordercloud/headstart-sdk';
import { Router } from '@angular/router';
import { CatalogsTempService } from '@app-seller/shared/services/middleware-api/catalogs-temp.service';

@Component({
  selector: 'app-buyer-location-catalogs',
  templateUrl: './buyer-location-catalogs.component.html',
  styleUrls: ['./buyer-location-catalogs.component.scss'],
})
export class BuyerLocationCatalogs {
  buyerID = '';
  locationID = '';

  @Input()
  set locationUserGroup(locationUserGroup: any) {
    if(locationUserGroup && Object.keys(locationUserGroup)) {
      const routeUrl = this.router.routerState.snapshot.url;
      this.buyerID = routeUrl.split('/')[2];
      this.locationID = locationUserGroup?.ID;
      this.resetAssignments(locationUserGroup.xp.CatalogAssignments || []);
    }
  }
  @Input()
  catalogs: MarketplaceCatalog[] = [];
  @Input()
  isCreatingNew: boolean;
  @Output() assignmentsToAdd = new EventEmitter<MarketplaceCatalogAssignmentRequest>();

  locationCatalogAssignmentsEditable: string[] = [];
  locationCatalogAssignmentsStatic: string[] = [];
  addLocationCatalogAssignments: string[] = [];
  delLocationCatalogAssignments: string[] = [];
  catalogAssignments: MarketplaceCatalogAssignmentRequest = { CatalogIDs: [] };
  areChanges = false;
  dataIsSaving = false;

  constructor(private router: Router, private marketplaceCatalogService: CatalogsTempService) {}

  resetAssignments(assignments: string[]): void {
    this.locationCatalogAssignmentsEditable = assignments;
    this.locationCatalogAssignmentsStatic = assignments;
    this.checkForChanges();
  }

  checkForChanges(): void {
    this.addLocationCatalogAssignments = this.locationCatalogAssignmentsEditable.filter(
      l => !this.locationCatalogAssignmentsStatic.includes(l)
    );
    this.delLocationCatalogAssignments = this.locationCatalogAssignmentsStatic.filter(
      l => !this.locationCatalogAssignmentsEditable.includes(l)
    );
    this.catalogAssignments.CatalogIDs = this.locationCatalogAssignmentsEditable;
    this.assignmentsToAdd.emit(this.catalogAssignments);
    this.areChanges = !!this.delLocationCatalogAssignments.length || !!this.addLocationCatalogAssignments.length;
  }

  isAssigned(catalog: MarketplaceCatalog): boolean {
    return this.locationCatalogAssignmentsEditable.includes(catalog.ID);
  }

  toggleAssignment(catalog: MarketplaceCatalog): void {
    if (this.isAssigned(catalog)) {
      this.locationCatalogAssignmentsEditable = this.locationCatalogAssignmentsEditable.filter(c => c !== catalog.ID);
    } else {
      this.locationCatalogAssignmentsEditable = [...this.locationCatalogAssignmentsEditable, catalog.ID];
    }
    this.checkForChanges();
  }

  discardChanges(): void {
    this.resetAssignments(this.locationCatalogAssignmentsStatic);
  }

  async saveChanges(): Promise<void> {
    this.resetAssignments(this.locationCatalogAssignmentsEditable);
    await this.marketplaceCatalogService.setLocationAssignments(
      this.buyerID,
      this.locationID,
      this.locationCatalogAssignmentsEditable
    );
  }
}
