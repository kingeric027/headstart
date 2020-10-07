import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { PriceBreak, Spec } from 'ordercloud-javascript-sdk';
import { minBy as _minBy } from 'lodash';
import { SpecFormService } from '../spec-form/spec-form.service';


@Injectable({
    providedIn: 'root',
})
export class ProductDetailService {

    constructor(
        private specFormService: SpecFormService
    ) { }

    getProductPrice(priceBreaks: PriceBreak[], specs: Spec[], specForm: FormGroup, quantity: number): number {
        // In OC, the price per item can depend on the quantity ordered. This info is stored on the PriceSchedule as a list of PriceBreaks.
        // Find the PriceBreak with the highest Quantity less than the quantity ordered. The price on that price break
        // is the cost per item.
        if (!priceBreaks?.length) return;
        const startingBreak = _minBy(priceBreaks, 'Quantity');
        const selectedBreak = priceBreaks.reduce((current, candidate) => {
            return candidate.Quantity > current.Quantity && candidate.Quantity <= quantity ? candidate : current;
        }, startingBreak);

        // Take into account markups if they are applied which can increase price
        return specForm?.valid
            ? this.specFormService.getSpecMarkup(specs, selectedBreak, quantity || startingBreak.Quantity, specForm)
            : selectedBreak.Price * (quantity || startingBreak.Quantity);
    }

    getPercentSavings(actualPrice: number, basePrice: number): number {
        if (actualPrice === null || actualPrice === undefined) {
            return 0;
        }
        if (basePrice === null || basePrice === undefined) {
            return 0;
        }
        return parseInt(
            (((basePrice - actualPrice) / basePrice) * 100).toFixed(0), 10
        );
    }
}