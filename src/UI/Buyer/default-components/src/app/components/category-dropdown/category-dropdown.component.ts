import { Component, Input } from '@angular/core';
import { Category } from '@ordercloud/angular-sdk';
import { CategoryFilterPipe } from '../../pipes/category-dropdown.pipe';
import { ShopperContextService } from 'marketplace';

@Component({
	templateUrl: './category-dropdown.component.html',
	styleUrls: ['./category-dropdown.component.scss'],
	providers: [CategoryFilterPipe]
})
export class OCMCategoryDropdown {
	constructor(private categoryFilterPipe: CategoryFilterPipe, private context: ShopperContextService) { }

	@Input() showMenu;
	@Input() set categories(value: Category[]) {
		this.parentCategories = this.categoryFilterPipe.transform(value, 'parent');
		this.subCategories = this.categoryFilterPipe.transform(value, 'subCategory');
		this.subSubCategories = this.categoryFilterPipe.transform(value, 'subSubCategory');
	}

	parentCategories: Category[] = [];
	subCategories: Category[] = [];
	subSubCategories: Category[] = [];

	setActiveCategory(categoryID: string): void {
		this.context.router.toProductList({ categoryID });
	}

}