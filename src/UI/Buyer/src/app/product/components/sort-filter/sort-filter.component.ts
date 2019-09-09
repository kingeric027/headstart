import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ProductSortStrategy } from '@app-buyer/product/models/product-sort-strategy.enum';
import { each as _each } from 'lodash';
import { applicationConfiguration, AppConfig } from '@app-buyer/config/app.config';
import { ShopperContextService } from '@app-buyer/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'product-sort-filter',
  templateUrl: './sort-filter.component.html',
  styleUrls: ['./sort-filter.component.scss'],
})
export class SortFilterComponent implements OnInit {
  form: FormGroup;
  options: { label: string; value: string }[];

  constructor(
    private formBuilder: FormBuilder,
    private context: ShopperContextService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}

  ngOnInit() {
    this.options = this.getOptions();
    this.buildForm();
    this.context.productFilterActions.onFiltersChange((filters) => {
      this.setForm(filters.sortBy);
    });
  }

  private getOptions(): { label: string; value: string }[] {
    let options = [];
    _each(ProductSortStrategy, (strategyName, strategyVal) => {
      options = [...options, { label: strategyName, value: strategyVal }];
    });
    if (this.appConfig.premiumSearchEnabled) {
      // sorting by price is mocked by storing price on xp and
      // sorting by xp.Price
      // uncomment the below lines if your product model has xp.Price defined
      // options = [
      //   ...options,
      //   { label: 'Price: High to Low', value: '!xp.Price' },
      // ];
      // options = [
      //   ...options,
      //   { label: 'Price: Low to High', value: 'xp.Price' },
      // ];
    }
    return options;
  }

  private buildForm() {
    this.form = this.formBuilder.group({ strategy: null });
  }

  private setForm(sortBy: string) {
    sortBy = sortBy || null;
    this.form.setValue({ strategy: sortBy });
  }

  protected sortStrategyChanged() {
    const sortStrategy = this.form.get('strategy').value;
    this.context.productFilterActions.sortBy(sortStrategy);
  }
}
