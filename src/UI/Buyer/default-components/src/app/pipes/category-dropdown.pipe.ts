import { Pipe, PipeTransform } from '@angular/core';
import { Category } from '@ordercloud/angular-sdk';

@Pipe({
    name: 'categoryFilter',
    pure: false
})
export class CategoryFilterPipe implements PipeTransform {
    transform(categories: Category[], level: string): any {
        var parentCategories;
        var subCategories = [];
        var subSubCategories = [];
        if (!categories || !level) {
            return categories;
        }
        // returns a list of only top-level (parent) categories
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
        if (level === 'parent') {
            return parentCategories;
        } else if (level === 'subCategory') {
            return subCategories;
        } else if (level === 'subSubCategory') {
            return subSubCategories;
        }
    }
}
