export interface Filter {
    name: string;
    path: string;
    dataKey: string;
    sourceType: string;
    source: string;
    filterValues: any[];
}

export let buyerLocation: Filter[] = [
    { name: 'Buyer', path: 'BuyerID', dataKey: "ID", sourceType: 'oc', source: 'ocBuyerService', filterValues: [] },
    { name: 'Country', path: 'Country', dataKey: "abbreviation", sourceType: 'model', source: 'GeographyConfig', filterValues: [] },
    { name: 'State', path: 'State', dataKey: "abbreviation", sourceType: 'model', source: 'GeographyConfig', filterValues: [] },
];