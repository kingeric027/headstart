import { CancelReturnReason, TableHeader } from './cancel-return-translations.enum';

export class CancelReturnTableHeaders {
    productDetails: TableHeader;
    price: TableHeader;
    quantityOrdered: TableHeader;
    quantityReturnedCanceled: TableHeader;
    quantityAvailableForAction: TableHeader;
    returnCancelReason: TableHeader;
    selectReason: TableHeader;
}

export class CancelReturnTranslations {
    Headers: CancelReturnTableHeaders;
    AvailableReasons: CancelReturnReason[];
}