import { Input, Component, OnInit } from '@angular/core';
import { OCMComponent } from '../../base-component';
import { Supplier } from '@ordercloud/angular-sdk';

@Component({
  templateUrl: './supplier-card.component.html',
  styleUrls: ['./supplier-card.component.scss'],
})
export class OCMSupplierCard extends OCMComponent {
  @Input() supplier: Supplier = {};
  ngOnContextSet() {}
  shopSupplier(supplier: Supplier) {
    this.context.router.toProductList({ activeFacets: { Supplier: supplier.ID } });
  }
}
