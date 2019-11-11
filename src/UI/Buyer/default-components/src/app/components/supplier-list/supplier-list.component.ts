import { Component, OnInit, Input } from '@angular/core';
import { OCMComponent } from '../base-component';
import { ListSupplier } from '@ordercloud/angular-sdk';


@Component({
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.scss']
})
export class OCMSupplierList extends OCMComponent {

  @Input() suppliers: ListSupplier;

  ngOnContextSet() {
  }

}
