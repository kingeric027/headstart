export interface EnvironmentConfig {
  hostedApp: boolean
  sellerID: string
  clientID: string
  middlewareUrl: string
  cmsUrl: string
  appname: string
  translateBlobUrl: string
  blobStorageUrl: string
  orderCloudApiUrl: string
  orderCloudApiVersion: string
  buyerConfigs: Record<string, BuyerConfig>
  superProductFieldsToMonitor: string[]
}

export interface BuyerConfig {
  clientID: string
  buyerUrl: string
}

export enum Brand {
  'SELF_ESTEEM_BRANDS' = 'SELF_ESTEEM_BRANDS',
  'BRAND_WEAR_DESIGNS' = 'BRAND_WEAR_DESIGNS',
  'FAST_SIGNS' = 'FAST_SIGNS',
  'GO2PARTNERS' = 'GO2PARTNERS',
  'HEADSTART_DEMO' = 'HEADSTART_DEMO',
}

export enum Environment {
  'TEST' = 'TEST',
  'STAGING' = 'STAGING',
  'PRODUCTION' = 'PRODUCTION',
}
