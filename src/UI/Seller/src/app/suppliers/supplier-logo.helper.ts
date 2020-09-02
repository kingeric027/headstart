import { environment } from 'src/environments/environment';
import { MarketplaceSupplier } from '@ordercloud/headstart-sdk';

export const SUPPLIER_LOGO_PATH_STRATEGY = 'SUPPLIER_LOGO_PATH_STRATEGY';

export function getSupplierLogoSmallUrl(supplier: MarketplaceSupplier, sellerID: string): string {
  return `${environment.middlewareUrl}/assets/${sellerID}/suppliers/${supplier.ID}/thumbnail?size=s`;
}

export function getSupplierLogoMediumUrl(supplier: MarketplaceSupplier, sellerID: string): string {
  return `${environment.middlewareUrl}/assets/${sellerID}/suppliers/${supplier.ID}/thumbnail?size=m`;
}
