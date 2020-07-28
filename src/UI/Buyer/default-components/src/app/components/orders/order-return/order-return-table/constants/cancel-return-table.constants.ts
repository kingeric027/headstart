import { CancelReturnReason } from '../models/cancel-return-translations.enum';
import { TableHeader } from '../models/cancel-return-translations.enum';
import { CancelReturnTableHeaders } from '../models/cancel-return-translations.model';

export const returnHeaders: CancelReturnTableHeaders = {
        productDetails: TableHeader.productDetails,
        price: TableHeader.price,
        quantityOrdered: TableHeader.quantityOrdered,
        quantityReturnedCanceled: TableHeader.quantityToReturn,
        quantityAvailableForAction: TableHeader.quantityToReturn,
        returnCancelReason: TableHeader.returnReason,
        selectReason: TableHeader.selectReason
}

export const cancelHeaders: CancelReturnTableHeaders = {
        productDetails: TableHeader.productDetails,
        price: TableHeader.price,
        quantityOrdered: TableHeader.quantityOrdered,
        quantityReturnedCanceled: TableHeader.quantityToReturn,
        quantityAvailableForAction: TableHeader.quantityToReturn,
        returnCancelReason: TableHeader.returnReason,
        selectReason: TableHeader.selectReason
}

export const returnReasons: CancelReturnReason[] = [
    CancelReturnReason.IncorrectSizeOrStyle, 
    CancelReturnReason.IncorrectShipment, 
    CancelReturnReason.DoesNotMatchDescription, 
    CancelReturnReason.ProductDefective, 
    CancelReturnReason.PackagingDamaged, 
    CancelReturnReason.ReceivedExtraProduct, 
    CancelReturnReason.ArrivedLate, 
    CancelReturnReason.PurchaseMistake, 
    CancelReturnReason.NotNeeded, 
    CancelReturnReason.NotApproved, 
    CancelReturnReason.UnappliedDiscount, 
    CancelReturnReason.ProductMissing
];

export const cancelReasons: CancelReturnReason[] = [
    CancelReturnReason.IncorrectSizeOrStyle,
    CancelReturnReason.PurchaseMistake,
    CancelReturnReason.NotNeeded,
    CancelReturnReason.NotApproved,
    CancelReturnReason.UnappliedDiscount,
    //  Found different product,
    //  Product took too long to fulfill
]