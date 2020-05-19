import { Component, Input } from '@angular/core';
import { ListSpec, PriceSchedule } from '@ordercloud/angular-sdk';
import { minBy as _minBy } from 'lodash';
import { LineItem, MarketplaceMeProduct, ShopperContextService } from 'marketplace';
import { SpecFormService } from '../spec-form/spec-form.service';

@Component({
    templateUrl: `./grid-spec-form.component.html`,
})
export class OCMGridSpecForm {
    @Input() priceBreaks: object;
    @Input() priceSchedule: PriceSchedule;
    _specs: ListSpec;
    _product: MarketplaceMeProduct;
    quantity: number;
    selectedBreak: object;
    percentSavings: number;
    specOptions: any;
    lineItems: LineItem[] = [];
    lineTotals: number[] = [];
    totalPrice: number = 0;

    constructor(private specFormService: SpecFormService, private context: ShopperContextService) { }
    @Input() set product(value: MarketplaceMeProduct) {
        this._product = value;
    }
    @Input() set specs(value: ListSpec) {
        this._specs = value;
        this.getSpecOptions(value);
    }

    getSpecOptions(specs: ListSpec): void {
        // creates an object with each spec option and its values
        const obj = {};
        for (let i = 0; i < specs.Items.length; i++) {
            specs.Items[i].Options.forEach(option => {
                let name = specs.Items[i].Name.replace(/ /g, '')
                obj[name] ? obj[name].push(option.Value) : obj[name] = [option.Value];
            });
        }
        this.specOptions = this.getAllSpecCombinations(obj);
    }

    getAllSpecCombinations(specOptions: object): string[] {
        // returns an array containing every combination of spec values
        const arr = [];
        for (let key in specOptions) {
            arr.push(specOptions[key]);
        }
        if (arr.length == 1) return arr[0];
        else {
            const result = [];
            const combinations = this.getAllSpecCombinations(arr.slice(1));
            for (let i = 0; i < combinations.length; i++) {
                for (let j = 0; j < arr[0].length; j++) {
                    result.push(arr[0][j] + ', ' + combinations[i]);
                }
            }
            result.forEach(() => this.lineTotals.push(0));
            return result;
        }
    }

    changeQuantity(specs, event): void {
        const indexOfSpec = this.specOptions.indexOf(specs);
        specs = specs.split(',');
        specs = specs.map(x => x.replace(/\s/g, ''));
        const liSpecs = this.specFormService.getGridLineItemSpecs(this._specs, specs);
        const item = {
            Quantity: event.qty,
            Product: this._product,
            ProductID: this._product.ID,
            Specs: liSpecs
        };
        const i = this.lineItems.findIndex(obj => obj.Specs === specs);
        if (i > -1) this.lineItems[i] = item;
        else this.lineItems.push(item);
        this.lineTotals[indexOfSpec] = this.getLineTotal(event.qty, liSpecs);
        this.totalPrice = this.getTotalPrice();
    }

    getLineTotal(qty: number, specs: any): number {
        let markup = 0;
        let price = this.priceSchedule?.PriceBreaks[0].Price;
        specs.forEach(spec => markup += spec.Markup);
        return (markup + price) * qty;
    }

    async addToCart(): Promise<void> {
        for (let i = 0; i < this.lineItems.length; i++) {
            if (this.lineItems[i].Quantity > 0) {
                await this.context.order.cart.add(this.lineItems[i]);
            } else continue;
        }
        this.lineItems = [];
    }

    getPriceBreakRange(index: number): string {
        if (!this.priceSchedule?.PriceBreaks.length) return '';
        const priceBreaks = this.priceSchedule.PriceBreaks;
        const indexOfNextPriceBreak = index + 1;
        if (indexOfNextPriceBreak < priceBreaks.length) {
            return `${priceBreaks[index].Quantity} - ${priceBreaks[indexOfNextPriceBreak].Quantity - 1}`;
        } else {
            return `${priceBreaks[index].Quantity}+`;
        }
    }

    getTotalPrice(): number {
        return this.lineTotals.reduce((acc, curr) => { return acc + curr });
    }
}
