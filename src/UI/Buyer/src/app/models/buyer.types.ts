import { MarketplaceAddressBuyer, MarketplaceMeProduct, TaxCertificate } from "@ordercloud/headstart-sdk";

export interface BuyerLocationWithCert {
    location?: MarketplaceAddressBuyer
    certificate?: TaxCertificate
  }

  //    these can be replaced with sdk
export interface BuyerRequestForInfo {
    FirstName: string
    LastName: string
    Email: string
    Phone: string
    Comments: string
}

export interface ContactSupplierBody {
    Product: MarketplaceMeProduct
    BuyerRequest: BuyerRequestForInfo
}