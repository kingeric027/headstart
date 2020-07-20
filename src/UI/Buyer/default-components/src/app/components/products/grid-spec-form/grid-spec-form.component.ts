import { Component, Input } from '@angular/core';
import { Spec, PriceSchedule, ListPage } from 'ordercloud-javascript-sdk';
import { MarketplaceMeProduct, ShopperContextService } from 'marketplace';
import { SpecFormService, GridSpecOption } from '../spec-form/spec-form.service';
import { QtyChangeEvent } from '../quantity-input/quantity-input.component';
import { MarketplaceLineItem, SuperMarketplaceProduct } from 'marketplace-javascript-sdk';

@Component({
    templateUrl: `./grid-spec-form.component.html`,
})
export class OCMGridSpecForm {
    @Input() priceSchedule: PriceSchedule;
    _specs: ListPage<Spec>;
    _product: MarketplaceMeProduct;
    _superProduct: SuperMarketplaceProduct;
    specOptions: string[];
    lineItems: MarketplaceLineItem[] = [];
    lineTotals: number[] = [];
    totalPrice = 0;
    isAddingToCart = false;

    constructor(private specFormService: SpecFormService, private context: ShopperContextService) { }
    @Input() set superProduct(value: SuperMarketplaceProduct) {
        this._superProduct = value;
    }
    @Input() set product(value: MarketplaceMeProduct) {
        this._product = value;
    }
    @Input() set specs(value: ListPage<Spec>) {
        this._specs = value;
        this.getSpecOptions(value);
    }

    getSpecOptions(specs: ListPage<Spec>): void {
        // creates an object with each spec option and its values
        const obj = {};
        for (const spec of specs.Items) {
            for (const option of spec.Options) {
                const name = spec.Name.replace(/ /g, '');
                if (obj[name]) obj[name].push(option.Value);
                else obj[name] = [option.Value];
            }
        }
        this.specOptions = this.getAllSpecCombinations(obj);
    }

    getAllSpecCombinations(specOptions: object): string[] {
        // returns an array containing every combination of spec values
        const arr = [];
        for (const specName in specOptions) {
            if (specOptions.hasOwnProperty(specName)) {
                arr.push(specOptions[specName]);
            }
        }
        if (arr.length === 1) return arr[0];
        else {
            const result = [];
            const combinations = this.getAllSpecCombinations(arr.slice(1));
            for (const combination of combinations) {
                for (const optionValue of arr[0]) {
                    result.push(optionValue + ', ' + combination);
                }
            }
            result.forEach(() => this.lineTotals.push(0));
            return result;
        }
    }

    changeQuantity(specs: string, event: QtyChangeEvent): void {
        const indexOfSpec = this.specOptions.indexOf(specs);
        let specArray = specs.split(',');
        specArray = specArray.map(x => x.replace(/\s/g, ''));
        const item = {
            Quantity: event.qty,
            Product: this._product,
            ProductID: this._product.ID,
            Specs: this.specFormService.getGridLineItemSpecs(this._specs, specArray),
            xp: {
                LineItemImageUrl: this.specFormService.getLineItemImageUrl(this._superProduct)
            }
        };
        const i = this.lineItems.findIndex(li => JSON.stringify(li.Specs) === JSON.stringify(item.Specs));
        if (i === -1) this.lineItems.push(item);
        else this.lineItems[i] = item;
        this.lineTotals[indexOfSpec] = this.getLineTotal(event.qty, this.specFormService.getGridLineItemSpecs(this._specs, specArray));
        this.totalPrice = this.getTotalPrice();
    }

    getLineTotal(qty: number, options: GridSpecOption[]): number {
        let markup = 0;
        const price = this.priceSchedule?.PriceBreaks[0].Price;
        options.forEach(spec => markup += spec.Markup);
        return (markup + price) * qty;
    }

    async addToCart(): Promise<void> {
        const lineItems = this.lineItems.filter((li) => li.Quantity > 0);
        try {
            this.isAddingToCart = true;
            await this.context.order.cart.addMany(lineItems);
        } catch (ex) {
            this.isAddingToCart = false;
            throw ex;
        }
        this.isAddingToCart = false;
        this.lineItems = [];
    }

    getTotalPrice(): number {
        return this.lineTotals.reduce((acc, curr) => { return acc + curr });
    }
}
