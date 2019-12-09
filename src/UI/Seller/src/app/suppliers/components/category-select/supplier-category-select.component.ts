import { Component, Input, Output, EventEmitter } from '@angular/core';
import { SupplierCategoryConfig } from '../suppliers/supplier-table/supplier-table.component';
import { FormControl } from '@angular/forms';

interface SupplierCategorySelection {
  ServiceCategory: string;
  VendorLevel: string;
}

export const areAllCategoriesComplete = (categorySelections: SupplierCategorySelection[]): boolean => {
  return !categorySelections.some((category) => {
    return !category.ServiceCategory || !category.VendorLevel;
  });
};

export const areDuplicateCategories = (categorySelections: SupplierCategorySelection[]): boolean => {
  return categorySelections.some((selection) => isADuplicateCategory(selection, categorySelections));
};

export const isADuplicateCategory = (
  categorySelection: SupplierCategorySelection,
  categorySelections: SupplierCategorySelection[]
): boolean => {
  const categorySelectionsFlat = categorySelections.map(
    (selection) => `${selection.ServiceCategory}${selection.VendorLevel}`
  );
  return (
    categorySelectionsFlat.filter(
      (selectionFlat) => selectionFlat === `${categorySelection.ServiceCategory}${categorySelection.VendorLevel}`
    ).length > 1
  );
};

export const isSecondDuplicateCategory = (
  categorySelection: SupplierCategorySelection,
  categorySelections: SupplierCategorySelection[],
  index: number
): boolean => {
  const categorySelectionsFlat = categorySelections.map(
    (selection) => `${selection.ServiceCategory}${selection.VendorLevel}`
  );
  const indexOfFirstAppearanceOfCategory = categorySelectionsFlat.indexOf(
    `${categorySelection.ServiceCategory}${categorySelection.VendorLevel}`
  );
  return indexOfFirstAppearanceOfCategory < index;
};

@Component({
  selector: 'supplier-category-select-component',
  templateUrl: './supplier-category-select.component.html',
  styleUrls: ['./supplier-category-select.component.scss'],
})
export class SupplierCategorySelectComponent {
  canAddAnotherCategory = true;
  _categorySelectionsControl: FormControl;
  _categorySelections: SupplierCategorySelection[];

  isSecondDuplicateCategory = isSecondDuplicateCategory;
  areNoCategories = false;

  @Input()
  set categorySelectionsControl(value: FormControl) {
    this.updateCategoryValidation(value.value);
    this._categorySelectionsControl = value;
    this._categorySelections = value.value;
    this._categorySelectionsControl.valueChanges.subscribe((categorySelections) => {
      this._categorySelections = categorySelections;
      this.updateCategoryValidation(categorySelections);
    });
  }
  @Input()
  filterConfig: SupplierCategoryConfig;
  @Output()
  selectionsChanged = new EventEmitter();

  removeCategory(index: number): void {
    const newCategorySelection = JSON.parse(JSON.stringify(this._categorySelections));
    newCategorySelection.splice(index, 1);
    this.updateCategory(newCategorySelection);
  }

  addCategory(): void {
    const newCategorySelection = [...this._categorySelections, { ServiceCategory: '', VendorLevel: '' }];
    this.updateCategory(newCategorySelection);
  }

  makeSelection(event: any, field: string, index: number): void {
    const newCategorySelection = this._categorySelections;
    newCategorySelection[index][field] = event.target.value;
    this.updateCategory(newCategorySelection);
  }

  updateCategory(newCategorySelection: SupplierCategorySelection[]) {
    this.updateCategoryValidation(newCategorySelection);
    this.selectionsChanged.emit({ field: 'xp.Categories', value: newCategorySelection });
    this._categorySelectionsControl.setValue(newCategorySelection);
  }

  updateCategoryValidation(newCategorySelection: SupplierCategorySelection[]): void {
    this.areNoCategories = !newCategorySelection.length;
    this.canAddAnotherCategory = areAllCategoriesComplete(newCategorySelection);
  }
}
