import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Category, OcCategoryService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { BUYER_SUB_RESOURCE_LIST } from '@app-seller/buyers/components/buyers/buyer.service';

@Injectable({
  providedIn: 'root',
})
export class BuyerCategoryService extends ResourceCrudService<Category> {
  constructor(router: Router, activatedRoute: ActivatedRoute, private ocCategoryService: OcCategoryService) {
    super(router, activatedRoute, ocCategoryService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'categories');
  }

  async updateResource(resource: any): Promise<any> {
    await this.getResourceInformation(resource);
    return super.updateResource(resource);
  }

  async createNewResource(resource: any): Promise<any> {
    await this.getResourceInformation(resource);
    return super.createNewResource(resource);
  }

 async getResourceInformation(resource: any) {
    if (resource.ParentID) {
      const parentResourceID = this.getParentResourceID();
      let numberOfChecks = 0;
      const validDepth = await this.checkForDepth(parentResourceID, resource.ParentID, numberOfChecks);
      if (!validDepth) {
        throw {message: `The ${this.secondaryResourceLevel} cannot be saved this deep into a tree.  Please create this in a higher tier.`};
      } else {
        return true;
      }
    }
  }

  async checkForDepth (parentResourceID, resourceParentID, numberOfChecks) {
    numberOfChecks++;
    if (numberOfChecks === 3) {
      return false;
    }
    const parentOfResource = await this.ocCategoryService.Get(parentResourceID, resourceParentID).toPromise();
    return !parentOfResource.ParentID ? true : await this.checkForDepth(parentResourceID, parentOfResource.ParentID, numberOfChecks);
  }
}
