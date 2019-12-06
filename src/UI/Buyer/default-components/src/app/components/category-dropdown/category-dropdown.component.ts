import { Component, Input, OnChanges } from '@angular/core';
import { OCMComponent } from '../base-component';
import { Category } from '@ordercloud/angular-sdk';
import { CategoryFilterPipe } from '../../pipes/category-dropdown.pipe';

@Component({
	templateUrl: './category-dropdown.component.html',
	styleUrls: ['./category-dropdown.component.scss'],
	providers: [CategoryFilterPipe]
})
export class OCMCategoryDropdown extends OCMComponent implements OnChanges {
	constructor(private categoryFilterPipe: CategoryFilterPipe) { super() }
	@Input() showItems;
	@Input() categories;

	parentCategories: Category[] = [];
	subCategories: Category[] = [];
	subSubCategories: Category[] = [];

	ngOnChanges() {
		this.parentCategories = this.categoryFilterPipe.transform(this.categories, 'parent');
		this.subCategories = this.categoryFilterPipe.transform(this.categories, 'subCategory');
		this.subSubCategories = this.categoryFilterPipe.transform(this.categories, 'subSubCategory');
		console.log(this.subCategories, this.subSubCategories)
	}

	ngOnContextSet() { }

}