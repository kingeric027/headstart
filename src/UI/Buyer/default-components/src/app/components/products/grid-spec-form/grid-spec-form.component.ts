import { Component, Input } from '@angular/core';
import { ListSpec } from '@ordercloud/angular-sdk';
import { minBy as _minBy } from 'lodash';
import { LineItem, MarketplaceMeProduct, ShopperContextService } from 'marketplace';
import { SpecFormService } from '../spec-form/spec-form.service';

@Component({
    templateUrl: `./grid-spec-form.component.html`,
})
export class OCMGridSpecForm {
    _specs: ListSpec;
    _product: MarketplaceMeProduct;
    @Input() priceBreaks: object;
    quantity: number;
    selectedBreak: object;
    percentSavings: number;
    specOptions: any;
    _lineItems = [];
    lineItems: LineItem[] = [];
    lineTotals: number[] = [];
    unitPrices = [];

    constructor(private specFormService: SpecFormService, private context: ShopperContextService) { }
    @Input() set product(value: MarketplaceMeProduct) {
        this._product = value;
    }
    @Input() set specs(value: ListSpec) {
        this._specs = value;
        this.getSpecOptions(value);
    }

    getSpecOptions(specs: ListSpec) {
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

    getAllSpecCombinations(specOptions: object) {
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

    changeQuantity(specs, event) {
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
        console.log(this.lineItems);
    }

    getLineTotal(qty: number, specs: any) {
        let markup = 0;
        let price = this._product?.PriceSchedule?.PriceBreaks[0].Price;
        specs.forEach(spec => markup += spec.Markup);
        return (markup + price) * qty;
    }

    getLineItem(lineItemID: string): LineItem {
        return this._lineItems.find(li => li.ID === lineItemID);
    }

    addToCart(): void {
        this.lineItems.forEach(lineItem => {
            if (lineItem.Quantity > 0) {
                this.context.order.cart.add(lineItem);
            }
        });
        this.lineItems = [];
    }

    getPriceBreakRange(index: number): string {
        if (!this._product.PriceSchedule?.PriceBreaks.length) return '';
        const priceBreaks = this._product.PriceSchedule.PriceBreaks;
        const indexOfNextPriceBreak = index + 1;
        if (indexOfNextPriceBreak < priceBreaks.length) {
            return `${priceBreaks[index].Quantity} - ${priceBreaks[indexOfNextPriceBreak].Quantity - 1}`;
        } else {
            return `${priceBreaks[index].Quantity}+`;
        }
    }

    getTotalPrice(): number {
        // In OC, the price per item can depend on the quantity ordered. This info is stored on the PriceSchedule as a list of PriceBreaks.
        // Find the PriceBreak with the highest Quantity less than the quantity ordered. The price on that price break
        // is the cost per item.
        if (!this._product.PriceSchedule?.PriceBreaks.length) return;

        const priceBreaks = this._product.PriceSchedule.PriceBreaks;
        this.priceBreaks = priceBreaks;
        const startingBreak = _minBy(priceBreaks, 'Quantity');
        const selectedBreak = priceBreaks.reduce((current, candidate) => {
            return candidate.Quantity > current.Quantity && candidate.Quantity <= this.quantity ? candidate : current;
        }, startingBreak);
        this.selectedBreak = selectedBreak;
        this.percentSavings = parseInt(
            (((priceBreaks[0].Price - selectedBreak.Price) / priceBreaks[0].Price) * 100).toFixed(0), 10
        );
        return this.specFormService.event.valid
            ? this.specFormService.getSpecMarkup(this._specs, selectedBreak, this.quantity || startingBreak.Quantity)
            : selectedBreak.Price * (this.quantity || startingBreak.Quantity);
    }
}
