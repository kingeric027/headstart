export interface PromotionXp {
  Type?: MarketplacePromoType;
  Value?: number;
  AppliesTo?: MarketplacePromoEligibility;
  SKUs?: string[];
  Supplier?: string;
  Automatic?: boolean;
  MinReq?: MarketplacePromoMinRequirement;
  MaxShipCost?: number;
}

export interface MarketplacePromoMinRequirement {
  Type?: MinRequirementType;
  Int?: number;
}

export enum MarketplacePromoType {
  Percentage = 'Percentage',
  FixedAmount = 'FixedAmount',
  FreeShipping = 'FreeShipping',
}

export enum MarketplacePromoEligibility {
  EntireOrder = 'EntireOrder',
  SpecificSupplier = 'SpecificSupplier',
  SpecificSKUs = 'SpecificSKUs'
}

export enum MinRequirementType {
  MinPurchase = 'MinPurchase',
  MinItemQty = 'MinItemQty',
}
