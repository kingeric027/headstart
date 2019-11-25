import { Injectable } from '@angular/core';
import { ICategories } from '../../shopper-context';
import { OcMeService, Category } from '@ordercloud/angular-sdk';
import { listAll } from '../../functions/listAll';

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class ProductCategoriesService implements ICategories {

    private activeCategoryID: string = undefined;
    private allCategories: Category[] = undefined;
    private categoryBreadCrumbs: Category[] = [];

    constructor(private ocMeService: OcMeService) {}

    async setCategories(): Promise<void> {
        // We will have to enforce a maximum depth of 3 in the Admin UI.
        this.allCategories = (await listAll(this.ocMeService, this.ocMeService.ListCategories, { depth: '3', pageSize: 100 })).Items;
    }

    get activeID(): string {
        return this.activeCategoryID;
    }

    get all(): Category[] {
        return this.allCategories;
    }

    get breadCrumbs(): Category[] {
        return this.categoryBreadCrumbs;
    }

    setActiveCategoryID(value: string) {
        this.activeCategoryID = value;
        this.categoryBreadCrumbs = this.buildCategoryCrumbs(this.allCategories, value);
    }

    private buildCategoryCrumbs(allCategories: Category[], activeCategoryID: string, progress = []): Category[] {
        if (!activeCategoryID || !activeCategoryID || allCategories.length < 1) {
            return progress;
        }
        const category = allCategories.find(cat => cat.ID === activeCategoryID);
        if (!category) return progress;
        progress.unshift(category);

        return this.buildCategoryCrumbs(allCategories, category.ParentID, progress);
    }
}
