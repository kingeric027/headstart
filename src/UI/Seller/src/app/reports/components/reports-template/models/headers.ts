export const buyerLocation = [
  { value: 'ID', path: 'ID' },
  { value: 'Street 1', path: 'Street1' },
  { value: 'Street 2', path: 'Street2' },
  { value: 'City', path: 'City' },
  { value: 'State', path: 'State' },
  { value: 'Zip', path: 'Zip' },
  { value: 'Country', path: 'Country' },
  { value: 'Company Name', path: 'CompanyName' },
  { value: 'Primary Contact Name', path: 'xp.PrimaryContactName' },
  { value: 'Email', path: 'xp.Email' },
  { value: 'Phone', path: 'Phone' },
  { value: 'Opening Date', path: 'xp.OpeningDate' },
  { value: 'Billing Number', path: 'xp.BillingNumber' },
  { value: 'Status', path: 'xp.Status' },
  { value: 'Legal Entity', path: 'xp.LegalEntity' },
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
  { value: 'Submitted Order Status', path: 'xp.SubmittedOrderStatus' },
  { value: 'Shipping Status', path: 'xp.ShippingStatus' },
  { value: 'Payment Method', path: 'xp.PaymentMethod' },
  { value: 'Billing Street 1', path: 'BillingAddress.Street1' },
  { value: 'Billing Street 2', path: 'BillingAddress.Street2' },
  { value: 'Billing City', path: 'BillingAddress.City' },
  { value: 'Billing State', path: 'BillingAddress.State' },
  { value: 'Billing Zip', path: 'BillingAddress.Zip' },
  { value: 'Billing Country', path: 'BillingAddress.Country' },
  { value: 'Billing Number', path: 'BillingAddress.xp.BillingNumber' },
  { value: 'Club Number', path: 'BillingAddress.xp.LocationID' },
  { value: 'User ID', path: 'FromUser.ID' },
  { value: 'Username', path: 'FromUser.Username' },
  { value: 'First Name', path: 'FromUser.FirstName' },
  { value: 'Last Name', path: 'FromUser.LastName' },
  { value: 'Email', path: 'FromUser.Email' },
  { value: 'Phone', path: 'FromUser.Phone' },
  { value: 'Shipping First Name', path: 'xp.ShippingAddress.FirstName' },
  { value: 'Shipping Last Name', path: 'xp.ShippingAddress.LastName' },
  { value: 'Shipping Street 1', path: 'xp.ShippingAddress.Street1' },
  { value: 'Shipping Street 2', path: 'xp.ShippingAddress.Street2' },
  { value: 'Shipping City', path: 'xp.ShippingAddress.City' },
  { value: 'Shipping State', path: 'xp.ShippingAddress.State' },
  { value: 'Shipping Zip', path: 'xp.ShippingAddress.Zip' },
  { value: 'Shipping Country', path: 'xp.ShippingAddress.Country' },
]

export const purchaseOrderDetail = [
  { value: 'ID', path: 'ID' },
  { value: 'Supplier ID', path: 'ToCompanyID' },
  { value: 'Order Date', path: 'DateSubmitted' },
  { value: 'Total', path: 'Total' },
  { value: 'Order Currency', path: 'xp.Currency' },
  { value: 'Submitted Order Status', path: 'xp.SubmittedOrderStatus' },
  { value: 'Shipping Status', path: 'xp.ShippingStatus' },
  { value: 'Payment Method', path: 'xp.PaymentMethod' },
  { value: 'User ID', path: 'FromUser.ID' },
  { value: 'Username', path: 'FromUser.Username' },
  { value: 'First Name', path: 'FromUser.FirstName' },
  { value: 'Last Name', path: 'FromUser.LastName' },
  { value: 'Email', path: 'FromUser.Email' },
  { value: 'Phone', path: 'FromUser.Phone' },
  { value: 'Shipping First Name', path: 'xp.ShippingAddress.FirstName' },
  { value: 'Shipping Last Name', path: 'xp.ShippingAddress.LastName' },
  { value: 'Shipping Street 1', path: 'xp.ShippingAddress.Street1' },
  { value: 'Shipping Street 2', path: 'xp.ShippingAddress.Street2' },
  { value: 'Shipping City', path: 'xp.ShippingAddress.City' },
  { value: 'Shipping State', path: 'xp.ShippingAddress.State' },
  { value: 'Shipping Zip', path: 'xp.ShippingAddress.Zip' },
  { value: 'Shipping Country', path: 'xp.ShippingAddress.Country' },
]

export const lineItemDetail = [
  { value: 'Order ID', path: 'MarketplaceOrder.ID' },
  { value: 'Supplier ID', path: 'MarketplaceLineItem.SupplierID' },
  { value: 'Order Date', path: 'MarketplaceOrder.DateSubmitted' },
  { value: 'Total', path: 'MarketplaceOrder.Total' },
  { value: 'Tax', path: 'MarketplaceOrder.TaxCost' },
  { value: 'Shipping/Freight', path: 'MarketplaceOrder.ShippingCost' },
  { value: 'Discount', path: 'MarketplaceOrder.PromotionDiscount' },
  { value: 'Subtotal', path: 'MarketplaceOrder.Subtotal' },
  { value: 'Order Currency', path: 'MarketplaceOrder.xp.Currency' },
  {
    value: 'Submitted Order Status',
    path: 'MarketplaceOrder.xp.SubmittedOrderStatus',
  },
  { value: 'Payment Method', path: 'MarketplaceOrder.xp.PaymentMethod' },
  { value: 'Line Item ID', path: 'MarketplaceLineItem.ID' },
  { value: 'Product ID', path: 'MarketplaceLineItem.ProductID' },
  { value: 'Product Name', path: 'MarketplaceLineItem.Product.Name' },
  { value: 'Variant ID', path: 'MarketplaceLineItem.Variant.ID' },
  { value: 'Product Price', path: 'MarketplaceLineItem.UnitPrice' },
  { value: 'Quantity Ordered', path: 'MarketplaceLineItem.Quantity' },
  { value: 'Tax Code', path: 'MarketplaceLineItem.Product.xp.Tax.Code' },
  { value: 'Is Resale?', path: 'MarketplaceLineItem.Product.xp.IsResale' },
  {
    value: 'Unit of Measure Qty',
    path: 'MarketplaceLineItem.Product.xp.UnitOfMeasure.Qty',
  },
  {
    value: 'Unit of Measure',
    path: 'MarketplaceLineItem.Product.xp.UnitOfMeasure.Unit',
  },
  {
    value: 'Has Variants?',
    path: 'MarketplaceLineItem.Product.xp.HasVariants',
  },
  { value: 'Ship Method', path: 'MarketplaceLineItem.xp.ShipMethod' },
  {
    value: 'Shipping First Name',
    path: 'MarketplaceLineItem.ShippingAddress.FirstName',
  },
  {
    value: 'Shipping Last Name',
    path: 'MarketplaceLineItem.ShippingAddress.LastName',
  },
  {
    value: 'Shipping Street 1',
    path: 'MarketplaceLineItem.ShippingAddress.Street1',
  },
  {
    value: 'Shipping Street 2',
    path: 'MarketplaceLineItem.ShippingAddress.Street2',
  },
  { value: 'Shipping City', path: 'MarketplaceLineItem.ShippingAddress.City' },
  {
    value: 'Shipping State',
    path: 'MarketplaceLineItem.ShippingAddress.State',
  },
  { value: 'Shipping Zip', path: 'MarketplaceLineItem.ShippingAddress.Zip' },
  {
    value: 'Shipping Country',
    path: 'MarketplaceLineItem.ShippingAddress.Country',
  },
  {
    value: 'Billing Street 1',
    path: 'MarketplaceOrder.BillingAddress.Street1',
  },
  {
    value: 'Billing Street 2',
    path: 'MarketplaceOrder.BillingAddress.Street2',
  },
  { value: 'Billing City', path: 'MarketplaceOrder.BillingAddress.City' },
  { value: 'Billing State', path: 'MarketplaceOrder.BillingAddress.State' },
  { value: 'Billing Zip', path: 'MarketplaceOrder.BillingAddress.Zip' },
  { value: 'Billing Country', path: 'MarketplaceOrder.BillingAddress.Country' },
  {
    value: 'Billing Number',
    path: 'MarketplaceLineItem.ShippingAddress.xp.BillingNumber',
  },
  {
    value: 'Club Number',
    path: 'MarketplaceLineItem.ShippingAddress.xp.LocationID',
  },
  { value: 'User ID', path: 'MarketplaceOrder.FromUser.ID' },
  { value: 'Username', path: 'MarketplaceOrder.FromUser.Username' },
  { value: 'First Name', path: 'MarketplaceOrder.FromUser.FirstName' },
  { value: 'Last Name', path: 'MarketplaceOrder.FromUser.LastName' },
  { value: 'Email', path: 'MarketplaceLineItem.ShippingAddress.xp.Email' },
  { value: 'Phone', path: 'MarketplaceOrder.FromUser.Phone' },
]
