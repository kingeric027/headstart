export enum CancelReturnReason {
    IncorrectSizeOrStyle = 'ORDERS.RETURN_REASONS.PURCHASED_INCORRECT_SIZE',
    IncorrectShipment = 'ORDERS.RETURN_REASONS.INCORRECT_PRODUCT_SHIPPED',
    DoesNotMatchDescription = 'ORDERS.RETURN_REASONS.PRODUCT_MATCH_DESCRIPTION',
    ProductDefective = 'ORDERS.RETURN_REASONS.PRODUCT_DEFECTIVE',
    PackagingDamaged = 'ORDERS.RETURN_REASONS.SHIPPING_BOX_DAMAGED',
    ReceivedExtraProduct = 'ORDERS.RETURN_REASONS.RECEIVED_EXTRA_PRODUCT',
    ArrivedLate = 'ORDERS.RETURN_REASONS.PRODUCT_ARRIVED_LATE',
    PurchaseMistake = 'ORDERS.RETURN_REASONS.PURCHASED_BY_MISTAKE',
    NotNeeded = 'ORDERS.RETURN_REASONS.PRODUCT_NOT_NEEDED',
    NotApproved = 'ORDERS.RETURN_REASONS.PURCHASE_NOT_APPROVED',
    UnappliedDiscount = 'ORDERS.RETURN_REASONS.DISCOUNT_NOT_APPLIED',
    ProductMissing = 'ORDERS.RETURN_REASONS.PRODUCT_MISSING',
    //  Found different product,
    //  Product took too long to fulfill
}


export const ReturnReasons = [
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

export const CancelReasons = [
    CancelReturnReason.IncorrectSizeOrStyle,
    CancelReturnReason.PurchaseMistake,
    CancelReturnReason.NotNeeded,
    CancelReturnReason.NotApproved,
    CancelReturnReason.UnappliedDiscount,
    //  Found different product,
    //  Product took too long to fulfill
]

export const returnColumns = [
    'select',
    'product',
    'id',
    'price',
    'quantityOrdered',
    'quantityReturned',
    'quantityToReturn',
    'returnReason' 
]

export const cancelColumns = [
    'select',
    'product',
    'id',
    'price',
    'quantityOrdered',
    'quantityCanceled',
    'quantityToCancel',
    'cancelReason'
]
