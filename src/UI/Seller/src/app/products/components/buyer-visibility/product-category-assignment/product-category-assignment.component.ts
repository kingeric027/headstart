import { Component, Input } from '@angular/core';
import { OcCategoryService, Category } from '@ordercloud/angular-sdk';


@Component({
  selector: 'product-category-assignment',
  templateUrl: './product-category-assignment.component.html',
  styleUrls: ['./product-category-assignment.component.scss'],
})
export class ProductCategoryAssignment {
  @Input()
  productID = '';
  @Input()
  selectedCategoryID = '';
  @Input()
  catalogID = '';
  @Input()
  set isEditing(value: boolean) {
    console.log(value);
    if (value) {
      this.buildCategoryTree();
    }
  }

  // array of categories in which the final category is the one the product is assigned to
  // and prior categories are the parents of the assigned category or grandparents
  assignedCategoryHierarchy: Category[] = [];
  categoryOptionTree: Category[][] = [];
  areSubCategories = false;
  subCategories: Category[] = [];
  showSubCategories = false;

  constructor(private ocCategoryService: OcCategoryService) {}

  async buildCategoryTree(): Promise<void> {
    await this.buildCategoryHierarchy();
    await this.buildCategoryOptionTree();
  }


  async checkForSubcategories(): Promise<void> {
    const subCategories = await this.ocCategoryService
      .List(this.catalogID, {
        depth: '1',
        pageSize: 100,
        filters: { parentID: this.selectedCategoryID },
      })
      .toPromise();

    this.areSubCategories = subCategories.Meta.TotalCount > 0;
    this.subCategories = subCategories.Items;
  }

  handleSelectionChange(event: any): void {
    this.selectCategory(event.value);
  }

  async selectCategory(categoryID: string): Promise<void> {
    this.selectedCategoryID = categoryID;
    this.subCategories = [];
    this.showSubCategories = false;
    await this.checkForSubcategories();
  }

  handleShowSubcategories(): void {
    this.categoryOptionTree = [...this.categoryOptionTree, this.subCategories];
    this.showSubCategories = true;
  }
}
