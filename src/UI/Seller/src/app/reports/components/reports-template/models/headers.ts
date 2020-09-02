export const buyerLocation = [
    { value: 'ID', path: 'ID' },
    { value: 'Street 1', path: 'Street1' },
    { value: 'Street 2', path: 'Street2' },
    { value: 'City', path: 'City' },
    { value: 'State', path: 'State' },
    { value: 'Zip', path: 'Zip' },
    { value: 'Country', path: 'Country' },
    { value: 'Company Name', path: 'CompanyName'},
    { value: 'Email', path: 'xp.Email' },
    { value: 'Phone', path: 'Phone' }
]

export const salesOrderDetail = [
    { value: 'ID', path: 'ID' },
    { value: 'Order Date', path: 'DateSubmitted' },
    { value: 'Total', path: 'Total' },
    { value: 'Tax', path: 'TaxCost' },
    { value: 'Shipping/Freight', path: 'ShippingCost' },
    { value: 'Discount', path: 'PromotionDiscount' },
    { value: 'Subtotal', path: 'Subtotal' },
    { value: 'Order Currency', path: 'xp.Currency' },
    { value: 'Order Status', path: 'Status' },
    { value: 'Shipping Status', path: 'xp.ShippingStatus' },
    { value: 'Payment Method', path: 'xp.PaymentMethod'},
    { value: 'Billing Street 1', path: 'BillingAddress.Street1' },
    { value: 'Billing Street 2', path: 'BillingAddress.Street2' },
    { value: 'Billing City', path: 'BillingAddress.City' },
    { value: 'Billing State', path: 'BillingAddress.State' },
    { value: 'Billing Zip', path: 'BillingAddress.Zip' },
    { value: 'User ID', path: 'FromUser.ID' },
    { value: 'Username', path: 'FromUser.Username' },
    { value: 'First Name', path: 'FromUser.FirstName' },
    { value: 'Last Name', path: 'FromUser.LastName' },
    { value: 'Email', path: 'FromUser.Email' },
    { value: 'Phone', path: 'FromUser.Phone' },
    //  TO-DO - Shipping Address Details will be needed, 
    //  and getNestedValue() and middleware report download will need to be refactored
    //  to accommodate their depth.
]