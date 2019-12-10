import { Component, Input, OnChanges } from '@angular/core';
import { Category, ListCategory } from '@ordercloud/angular-sdk';
import { CategoryFilterPipe } from '../../pipes/category-dropdown.pipe';

@Component({
	templateUrl: './category-dropdown.component.html',
	styleUrls: ['./category-dropdown.component.scss'],
	providers: [CategoryFilterPipe]
})
export class OCMCategoryDropdown {
	constructor(private categoryFilterPipe: CategoryFilterPipe) { }
	@Input() showMenu;
	@Input() set categories(value: Category[]) {
		this.parentCategories = this.categoryFilterPipe.transform(value, 'parent');
		this.subCategories = this.categoryFilterPipe.transform(value, 'subCategory');
		this.subSubCategories = this.categoryFilterPipe.transform(value, 'subSubCategory');
	}

	parentCategories: Category[] = [];
	subCategories: Category[] = [];
	subSubCategories: Category[] = [];

}