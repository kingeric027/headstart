import { Category, OcCategoryService } from '@ordercloud/angular-sdk';

export class CategoryExplorer {
  // original list of options, for discarding changes
  topLevelOptions: Category[] = [];
  midLevelStaticOptions: Category[] = [];
  lowLevelStaticOptions: Category[] = [];

  // currentcOptions of options being show
  // top level options are always static
  midLevelDynamicOptions: Category[] = [];
  lowLevelDynamicOptions: Category[] = [];

  // original category selection and current selection
  topLevelStaticSelection: Category = {};
  midLevelStaticSelection: Category = {};
  lowLevelStaticSelection: Category = {};

  topLevelDynamicSelection: Category = {};
  midLevelDynamicSelection: Category = {};
  lowLevelDynamicSelection: Category = {};

  categoryIDStatic = '';
  categoryIDDynamic = '';

  constructor(private ocCategoryService: OcCategoryService, private catalogID: string, private categoryID = '') {
    this.categoryIDStatic = categoryID;
    this.getData();
  }

  async getData(): Promise<void> {
    await this.buildCategoryHierarchy();
    await this.buildCategoryOptionTree();
  }

  async buildCategoryOptionTree(): Promise<void> {
    const categoryListRequests = [
      this.ocCategoryService.List(this.catalogID, { depth: '1', pageSize: 100 }).toPromise(),
      this.ocCategoryService
        .List(this.catalogID, { depth: '2', pageSize: 100, filters: { ParentID: c.ID } })
        .toPromise(),
      this.ocCategoryService
        .List(this.catalogID, { depth: '3', pageSize: 100, filters: { ParentID: c.ID } })
        .toPromise(),
    ];

    const categoryListResponses = await Promise.all(categoryListRequests);
    this.categoryOptionTree = categoryListResponses.map(c => c.Items);
  }

  async buildCategoryHierarchy(parentID = ''): Promise<void> {
    if (!this.categoryIDStatic) return;
    const categoryHierachy = await this.addToCategoryHierarchy([]);

    this.topLevelDynamicSelection = categoryHierachy[0];
    this.topLevelStaticSelection = categoryHierachy[0];

    if (categoryHierachy.length > 1) {
      this.midLevelDynamicSelection = categoryHierachy[1];
      this.midLevelStaticSelection = categoryHierachy[1];
    }

    if (categoryHierachy.length > 2) {
      this.lowLevelDynamicSelection = categoryHierachy[2];
      this.lowLevelStaticSelection = categoryHierachy[2];
    }
  }

  async addToCategoryHierarchy(currentTree = [], parentID = ''): Promise<Category[]> {
    // recursive function to add currently selected category and all parents to an array

    const category = await this.ocCategoryService.Get(this.catalogID, parentID || this.selectedCategoryID).toPromise();
    const updatedTree = [category, ...currentTree];
    if (category.ParentID) {
      await this.addToCategoryHierarchy(category.ParentID);
    } else {
      return;
    }
  }
}
