import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Category, OcCategoryService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { BUYER_SUB_RESOURCE_LIST } from '@app-seller/buyers/components/buyers/buyer.service';
import { CurrentUserService } from '../current-user/current-user.service';

@Injectable({
  providedIn: 'root',
})
export class BuyerCategoryService extends ResourceCrudService<Category> {
  constructor(router: Router, activatedRoute: ActivatedRoute, private ocCategoryService: OcCategoryService, 
    currentUserService: CurrentUserService) {
    super(router, activatedRoute, ocCategoryService, currentUserService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'categories');
  }

  async updateResource(originalID: string, resource: Category): Promise<Category> {
    await this.getResourceInformation(resource);
    return super.updateResource(originalID, resource);
  }

  async createNewResource(resource: Category): Promise<Category> {
    await this.getResourceInformation(resource);
    return super.createNewResource(resource);
  }

 async getResourceInformation(resource: Category): Promise<boolean> {
    if (resource.ParentID) {
      const parentResourceID = this.getParentResourceID();
      let numberOfChecks = 0;
      const validDepth = await this.checkForDepth(parentResourceID, resource.ParentID, numberOfChecks);
      if (!validDepth) {
        throw {message: `Categories cannot be saved this deep into a tree.  Please create this in a higher tier.`};
      } else {
        return true;
      }
    }
  }

  async checkForDepth (parentResourceID: string, resourceParentID: string, numberOfChecks: number): Promise<boolean> {
    numberOfChecks++;
    if (numberOfChecks === 3) {
      return false;
    }
    const parentOfResource = await this.ocCategoryService.Get(parentResourceID, resourceParentID).toPromise();
    return !parentOfResource.ParentID ? true : await this.checkForDepth(parentResourceID, parentOfResource.ParentID, numberOfChecks);
  }
}
