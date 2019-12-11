import { Component, Input } from '@angular/core';
import { Category } from '@ordercloud/angular-sdk';
import { ShopperContextService } from 'marketplace';

@Component({
	templateUrl: './category-dropdown.component.html',
	styleUrls: ['./category-dropdown.component.scss']
})
export class OCMCategoryDropdown {
	constructor(private context: ShopperContextService) { }

	@Input() set categories(value: Category[]) {
		this.parentCategories = this.assignCategories(value, 'parent');
		this.subCategories = this.assignCategories(value, 'subCategory');
		this.subSubCategories = this.assignCategories(value, 'subSubCategory');
	}

	parentCategories: Category[] = [];
	subCategories: Category[] = [];
	subSubCategories: Category[] = [];

	assignCategories(categories: Category[], level: string): any {
		var parentCategories;
		var subCategories = [];
		var subSubCategories = [];

		if (!categories || !level) return categories;

		parentCategories = categories.filter(category => category.ParentID === null);

		categories.forEach(category => {
			for (let i = 0; i < parentCategories.length; i++) {
				if (parentCategories[i].ID === category.ParentID) {
					subCategories.push(category);
				}
			}
		});

		categories.forEach(category => {
			for (let i = 0; i < subCategories.length; i++) {
				if (subCategories[i].ID === category.ParentID) {
					subSubCategories.push(category);
				}
			}
		});

		if (level === 'parent') return parentCategories;
		else if (level === 'subCategory') return subCategories;
		else if (level === 'subSubCategory') return subSubCategories;
	}

	setActiveCategory(categoryID: string): void {
		this.context.router.toProductList({ categoryID });
	}

}