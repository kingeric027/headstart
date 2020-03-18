import { Injectable } from '@angular/core';
import { OcMeService, BuyerAddress, ListBuyerAddress } from '@ordercloud/angular-sdk';

export interface ICurrentUserAddress {
    list(args: ListArgs): Promise<ListBuyerAddress>;
    listShipping(args: ListArgs): Promise<ListBuyerAddress>;
    listBilling(args: ListArgs): Promise<ListBuyerAddress>;
    create(address: BuyerAddress): Promise<BuyerAddress>;
    edit(addressID: string, address: BuyerAddress): Promise<BuyerAddress>;
    delete(addressID: string): Promise<void>;
}

@Injectable({
    providedIn: 'root',
})
export class CurrentUserAddressService implements ICurrentUserAddress  {

    constructor(private ocMeService: OcMeService) {}

    async list(args: ListArgs): Promise<ListBuyerAddress> {
        return this.ocMeService.ListAddresses(args).toPromise();
    }

    async listBuyerLocations(args: ListArgs): Promise<ListBuyerAddress> {
        args.filters = { ...args.filters, Editable: 'false' };
        return this.list(args);
    }

    async listShipping(args: ListArgs): Promise<ListBuyerAddress> {
        args.filters = { ...args.filters, Shipping: 'true' };
        return this.list(args);
    }

    async listBilling(args: ListArgs): Promise<ListBuyerAddress> {
        args.filters = { ...args.filters, Billing: 'true' };
        return this.list(args);
    }

    async create(address: BuyerAddress): Promise<BuyerAddress> {
        return this.ocMeService.CreateAddress(address).toPromise();
    }

    async edit(addressID: string, address: BuyerAddress): Promise<BuyerAddress> {
        return this.ocMeService.SaveAddress(addressID, address).toPromise();
    }

    async delete(addressID: string): Promise<void> {
        return this.ocMeService.DeleteAddress(addressID).toPromise();
    }
}

export interface ListArgs {
    search?: string;
    searchOn?: string;
    sortBy?: string;
    page?: number;
    pageSize?: number;
    filters?: {
        [key: string]: string | string[];
    };
}
