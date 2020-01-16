import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, OnInit } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
import { Address, ListAddress, OcSupplierAddressService, MeUser, OcAdminAddressService } from '@ordercloud/angular-sdk';
import { SupplierAddressService } from '@app-seller/shared/services/supplier/supplier-address.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { FileHandle } from '@app-seller/shared/directives/dragDrop.directive';
import { UserContext } from '@app-seller/config/user-context';
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
  addresses: ListAddress;
  files: FileHandle[] = [];

  constructor(
    private supplierAddressService: SupplierAddressService,
    private currentUserService: CurrentUserService,
    private ocSupplierAddressService: OcSupplierAddressService,
    private ocAdminAddressService: OcAdminAddressService
  ) {}

  ngOnInit() {
    // TODO: Eventually move to a resolve so that they are there before the component instantiates.
    this.getAddresses();
  }

  async getAddresses(): Promise<void> {
    const context: UserContext = await this.currentUserService.getUserContext();
    context.Me.Supplier
      ? (this.addresses = await this.ocSupplierAddressService.List(context.Me.Supplier.ID).toPromise())
      : (this.addresses = await this.ocAdminAddressService.List().toPromise());
  }

  updateResourceFromEvent(event: any, field: string): void {
    this.updateResource.emit({ value: event.target.value, field });
  }

  // Image uploading functions
  filesDropped(files: FileHandle[]): void {
    this.files = files;
  }

  upload(): void {
    console.log(`UPLOAD ${this.files} to blob`);
  }
}
