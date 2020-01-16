import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, OnInit } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
import { Address, ListAddress, OcSupplierAddressService, MeUser } from '@ordercloud/angular-sdk';
import { SupplierAddressService } from '@app-seller/shared/services/supplier/supplier-address.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
@Component({
  selector: 'app-product-edit',
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.scss'],
})
export class ProductEditComponent implements OnInit {
  @Input()
  resourceForm: FormGroup;
  @Input()
  filterConfig;
  @Output()
  updateResource = new EventEmitter<any>();
  hasVariations = false;
  @Input()
  supplierAddresses: ListAddress;

  constructor(
    private supplierAddressService: SupplierAddressService,
    private currentUserService: CurrentUserService,
    private ocSupplierAddressService: OcSupplierAddressService
  ) {}

  ngOnInit() {
    // TODO: Eventually move to a resolve so that they are there before the component instantiates.
    this.getSupplierAddresses();
  }

  async getSupplierAddresses(): Promise<void> {
    const user: MeUser = await this.currentUserService.getUser();
    this.supplierAddresses = await this.ocSupplierAddressService.List(user.Supplier.ID).toPromise();
  }

  updateResourceFromEvent(event: any, field: string): void {
    this.updateResource.emit({ value: event.target.value, field });
  }
}
