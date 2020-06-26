import { Component, Input } from '@angular/core';
import { MarketplaceUserGroup, MarketplaceCatalog } from 'marketplace-javascript-sdk';
import { Router } from '@angular/router';
import { CatalogsTempService } from '@app-seller/shared/services/middleware-api/catalogs-temp.service';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';

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
      if (this.buyerID !== REDIRECT_TO_FIRST_PARENT) {
        this.getCatalogs();
      }
      this.resetAssignments(locationUserGroup.xp.CatalogAssignments || []);
    }
  }

  catalogs: MarketplaceCatalog[] = [];
  locationCatalogAssignmentsEditable: string[] = [];
  locationCatalogAssignmentsStatic: string[] = [];
  addLocationCatalogAssignments: string[] = [];
  delLocationCatalogAssignments: string[] = [];
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
    this.areChanges = !!this.delLocationCatalogAssignments.length || !!this.addLocationCatalogAssignments.length;
  }

  async getCatalogs(): Promise<void> {
    const catalogsResponse = await this.marketplaceCatalogService.list(this.buyerID);
    this.catalogs = catalogsResponse.Items;
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
