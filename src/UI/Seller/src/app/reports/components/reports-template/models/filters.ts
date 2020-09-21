export interface Filter {
  name: string;
  path: string;
  nestedDataPath?: string;
  dataKey?: string;
  sourceType: string;
  source: string;
  filterValues: any[];
}

export let buyerLocation: Filter[] = [
  { name: 'Buyer', 
    path: 'BuyerID', 
    dataKey: 'ID', 
    sourceType: 'oc', 
    source: 'ocBuyerService', 
    filterValues: [] },
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
];

export let salesOrderDetail: Filter[] = [
  {
    name: 'Order Status',
    path: 'Status',
    sourceType: 'model',
    source: 'OrderStatus',
    filterValues: [],
  },
  {
    name: 'Order Type',
    path: 'OrderType',
    sourceType: 'model',
    source: 'OrderType',
    filterValues: [],
  }
];

export let purchaseOrderDetail: Filter[] = [
  {
    name: 'Order Status',
    path: 'Status',
    sourceType: 'model',
    source: 'OrderStatus',
    filterValues: [],
  },
  {
    name: 'Order Type',
    path: 'OrderType',
    sourceType: 'model',
    source: 'OrderType',
    filterValues: [],
  }
];

export let lineItemDetail: Filter[] = [
  {
    name: 'Order Status',
    path: 'Status',
    sourceType: 'model',
    source: 'OrderStatus',
    filterValues: [],
  },
  {
    name: 'Order Type',
    path: 'OrderType',
    sourceType: 'model',
    source: 'OrderType',
    filterValues: [],
  }
];
