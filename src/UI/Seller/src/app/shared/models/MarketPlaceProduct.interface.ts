import { Product, PriceSchedule } from '@ordercloud/angular-sdk';

export const DRAFT = 'DRAFT';
export type DRAFT = typeof DRAFT;

export const PUBLISHED = 'PUBLISHED';
export type PUBLISHED = typeof PUBLISHED;

export type ObjectStatus = DRAFT | PUBLISHED;

// initial implementation of the MarketPlaceProduct model that has a corresponding
// model in the C# product
// making all non-product fields optional right now, could change later

export interface MarketPlaceProduct extends Product<MarketplaceProductXp> {
  Shipping?: ProductShipping;
  PriceSchedule?: PriceSchedule;
  HasVariants?: boolean;
  Notes?: string;
  UnitOfMeasure?: UnitOfMeasure;
  Status?: ObjectStatus;
}

export interface SuperMarketplaceProduct {
  Product: Product<MarketplaceProductXp>;
  PriceSchedule: PriceSchedule;
}

interface ProductShipping {
  ShipWeight: number;
  ShipHeight: number;
  ShipWidth: number;
  ShipLength: number;
}

interface UnitOfMeasure {
  Qty: number;
  Unit: string;
}

interface TaxProperties {
  Category: string;
  Code: string;
  Description: string;
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
  ProductType: string;
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
