export interface Filter {
  name: string
  path: string
  nestedDataPath?: string
  dataKey?: string
  sourceType: string
  source: string
  filterValues: any[]
}

export const buyerLocation: Filter[] = [
  {
    name: 'Buyer',
    path: 'BuyerID',
    dataKey: 'ID',
    sourceType: 'oc',
    source: 'ocBuyerService',
    filterValues: [],
  },
  {
    name: 'Country',
    path: 'Country',
    dataKey: 'abbreviation',
    sourceType: 'model',
    source: 'GeographyConfig',
    filterValues: [],
  },
  {
    name: 'State',
    path: 'State',
    dataKey: 'abbreviation',
    sourceType: 'model',
    source: 'GeographyConfig',
    filterValues: [],
  },
]

export const salesOrderDetail: Filter[] = [
  {
    name: 'Submitted Order Status',
    path: 'SubmittedOrderStatus',
    sourceType: 'model',
    source: 'SubmittedOrderStatus',
    filterValues: [],
  },
  {
    name: 'Order Type',
    path: 'OrderType',
    sourceType: 'model',
    source: 'OrderType',
    filterValues: [],
  },
  {
    name: 'Country',
    path: 'Country',
    dataKey: 'abbreviation',
    sourceType: 'model',
    source: 'GeographyConfig',
    filterValues: [],
  },
]

export const purchaseOrderDetail: Filter[] = [
  {
    name: 'Submitted Order Status',
    path: 'SubmittedOrderStatus',
    sourceType: 'model',
    source: 'SubmittedOrderStatus',
    filterValues: [],
  },
  {
    name: 'Order Type',
    path: 'OrderType',
    sourceType: 'model',
    source: 'OrderType',
    filterValues: [],
  },
]

export const lineItemDetail: Filter[] = [
  {
    name: 'Submitted Order Status',
    path: 'SubmittedOrderStatus',
    sourceType: 'model',
    source: 'SubmittedOrderStatus',
    filterValues: [],
  },
  {
    name: 'Order Type',
    path: 'OrderType',
    sourceType: 'model',
    source: 'OrderType',
    filterValues: [],
  },
  {
    name: 'Country',
    path: 'Country',
    dataKey: 'abbreviation',
    sourceType: 'model',
    source: 'GeographyConfig',
    filterValues: [],
  },
]
