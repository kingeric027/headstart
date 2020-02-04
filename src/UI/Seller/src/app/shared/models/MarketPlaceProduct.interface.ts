import { Product, PriceSchedule } from '@ordercloud/angular-sdk';

export const DRAFT = 'DRAFT';
export type DRAFT = typeof DRAFT;

export const PUBLISHED = 'PUBLISHED';
export type PUBLISHED = typeof PUBLISHED;

export type ObjectStatus = DRAFT | PUBLISHED;

// initial implementation of the MarketPlaceProduct model that has a corresponding
// model in the C# product
// making all non-product fields optional right now, could change later

export interface MarketPlaceProduct extends Product<MarketPlaceProductXp> {
  Shipping?: ProductShipping;
  PriceSchedule?: PriceSchedule;
  HasVariants?: boolean;
  Notes?: string;
  UnitOfMeasure?: UnitOfMeasure;
  Status?: ObjectStatus;
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

interface MarketPlaceProductXp {
  TaxCode: MarketPlaceProductTaxCode;
  Data: any;
  Images: MarketPlaceProductImage[];
}

export interface MarketPlaceProductTaxCode {
  Category: string;
  Code: string;
  Description: string;
}
export interface MarketPlaceProductImage {
  URL: string;
  Tags: string[];
}
