import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { ListSpec } from '@ordercloud/angular-sdk';
import { minBy as _minBy } from 'lodash';
import { MarketplaceMeProduct, ShopperContextService, LineItem } from 'marketplace';
import { SpecFormService } from '../spec-form/spec-form.service';
import { SpecFormTextAreaComponent } from '../spec-form/spec-form-textarea/spec-form-textarea.component';

@Component({
    templateUrl: `./grid-spec-form.component.html`,
})
export class OCMGridSpecForm {
    _specs: ListSpec;
    @Output() specFormChange: EventEmitter<any> = new EventEmitter<any>();
    _product: MarketplaceMeProduct;
    @Input() priceBreaks: object;
    quantity: number;
    price: number;
    priceBreakRange: string[];
    selectedBreak: object;
    percentSavings: number;
    specOptions: object;
    _lineItems = [];
    lineItems: LineItem[] = [];
    _specArray = [];
    specObject: object;
    constructor(private specFormService: SpecFormService, private context: ShopperContextService) { }
    @Input() set product(value: MarketplaceMeProduct) {
        this._product = value;
    }
    @Input() set specs(value: ListSpec) {
        this._specs = value;
        this.getSpecOptions(value);
    }

    getSpecOptions(specs: ListSpec) {
        const obj = {};
        for (let i = 0; i < specs.Items.length; i++) {
            specs.Items[i].Options.forEach(option => {
                let name = specs.Items[i].Name.replace(/ /g, '')
                obj[name] ? obj[name].push(option.Value) : obj[name] = [option.Value];
            });
        }
        this.specObject = obj;
        this.specOptions = this.getAllSpecCombinations(obj);
    }

    getAllSpecCombinations(specOptions: object) {
        const arr = [];
        for (let key in specOptions) {
            arr.push(specOptions[key]);
        }
        if (arr.length == 1) {
            return arr[0];
        } else {
            const result = [];
            const combinations = this.getAllSpecCombinations(arr.slice(1));
            for (let i = 0; i < combinations.length; i++) {
                for (let j = 0; j < arr[0].length; j++) {
                    result.push(arr[0][j] + ', ' + combinations[i]);
                }
            }
            console.log(result);
            return result;
        }
    }

    changeQuantity(specs, event): void {
        specs = specs.split(',');
        specs = specs.map(x => x.replace(/\s/g, ''));
        var item = {
            Quantity: event.target.value,
            Product: this._product,
            ProductID: this._product.ID,
            Specs: this.specFormService.getGridLineItemSpecs(this._specs, specs)
        };
        const i = this.lineItems.findIndex(obj => obj.Specs === specs);
        if (i > -1) this.lineItems[i] = item;
        else this.lineItems.push(item);
    }

    getLineItem(lineItemID: string): LineItem {
        return this._lineItems.find(li => li.ID === lineItemID);
    }

    addToCart(): void {
        this.lineItems.forEach(lineItem => {
            if (lineItem.Quantity > 0) {
                this.context.order.cart.add({
                    ProductID: this._product.ID,
                    Quantity: lineItem.Quantity,
                    Specs: lineItem.Specs
                });
            }
            console.log('li', lineItem);
        })
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

    getLineTotal(qty: number, specs: any): number {

        return 0;
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
