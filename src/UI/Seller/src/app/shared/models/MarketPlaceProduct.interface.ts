import { Product, PriceSchedule } from '@ordercloud/angular-sdk';

// initial implementation of the MarketPlaceProduct model that has a corresponding
// model in the C# product
// making all non-product fields optional right now, could change later

export interface SuperMarketplaceProduct {
  Product: MarketplaceProduct;
  PriceSchedule: PriceSchedule;
}

export type MarketplaceProduct = Product<MarketplaceProductXp>;

interface UnitOfMeasure {
  Qty: number;
  Unit: string;
}

interface TaxProperties {
  Category: string;
  Code: string;
  Description: string;
}

export enum ProductType {
  Standard = 'Standard',
  Quote = 'Quote',
}

interface FacetDictionary {
  [key: string]: string[];
}

interface MarketplaceProductXp {
  // DO NOT DELETE //
  IntegrationData: any;
  Facets: FacetDictionary;
  Images: MarketPlaceProductImage[];
  // DO NOT DELETE //
  Status: ObjectStatus;
  HasVariants: boolean;
  Note: string;
  Tax: TaxProperties;
  UnitOfMeasure: UnitOfMeasure;
  ProductType: ProductType;
  StaticContent: MarketPlaceProductStaticContent[];
}

export interface MarketPlaceProductStaticContent {
  URL: string;
  Title: string;
}

export enum ObjectStatus {
  Draft = 'Draft',
  Published = 'Published',
}

export interface MarketPlaceProductTaxCode {
  Category: string;
  Code: string;
  Description: string;
}
export interface MarketPlaceProductImage {
  URL: string;
  Tag: string[];
}
