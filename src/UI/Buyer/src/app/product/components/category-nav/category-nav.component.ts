import { Component, OnInit, Input } from '@angular/core';
import { ListCategory, Category } from '@ordercloud/angular-sdk';
import { ITreeOptions } from 'angular-tree-component';
import { CategoryTreeNode } from '@app-buyer/product/models/category-tree-node.class';
import { ShopperContextService } from '@app-buyer/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'product-category-nav',
  templateUrl: './category-nav.component.html',
  styleUrls: ['./category-nav.component.scss'],
})
export class CategoryNavComponent implements OnInit {
  @Input() categories: ListCategory;
  categoryTree: CategoryTreeNode[];
  treeOptions: ITreeOptions = this.buildTreeOptions();
  private activeCategoryID: string;

  constructor(private context: ShopperContextService) {}

  ngOnInit() {
    this.categoryTree = this.buildCategoryTree(this.categories.Items);
    this.context.productFilterActions.onFiltersChange((filters) => {
      this.activeCategoryID = filters.categoryID;
    });
  }

  buildTreeOptions(): ITreeOptions {
    return {
      nodeClass: (node: CategoryTreeNode) => {
        return this.activeCategoryID === node.id ? 'font-weight-bold' : null;
      },
      actionMapping: {
        mouse: {
          click: (_tree, _node, _$event) => {
            this.context.productFilterActions.filterByCategory(_node.id);
          },
        },
      },
      animateExpand: true,
    };
  }

  buildCategoryTree(categories: Category[]): CategoryTreeNode[] {
    // key is ID, value is Node
    const nodeDictionary = this.buildNodeDictionary(categories);
    categories.forEach((category) => this.setParentsAndChildren(nodeDictionary, category.ID));
    // Return all top-level nodes in order
    return categories.map((category) => nodeDictionary[category.ID]).filter((x) => !x.parent);
  }

  private setParentsAndChildren(nodeDictionary: any, categoryID: string) {
    if (!nodeDictionary[categoryID].category.ParentID || !nodeDictionary[nodeDictionary[categoryID].category.ParentID]) {
      // category is not a child node
      return;
    }

    nodeDictionary[nodeDictionary[categoryID].category.ParentID].children.push(nodeDictionary[categoryID]);
    nodeDictionary[categoryID].parent = nodeDictionary[nodeDictionary[categoryID].category.ParentID];
  }

  // returns an object with a key for each categoryID
  private buildNodeDictionary(categories: Category[]): any {
    const nodeDict = {};
    categories.forEach((cat: Category) => {
      nodeDict[cat.ID] = new CategoryTreeNode(cat);
    });
    return nodeDict;
  }
}
