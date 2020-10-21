export interface Environment {
    hostedApp: boolean;
    sellerID: string;
    clientID: string;
    middlewareUrl: string;
    appname: string;
    translateBlobUrl: string;
    blobStorageUrl: string;
    orderCloudApiUrl: string;
    orderCloudApiVersion: string;
    buyerConfigs: Record<string, BuyerConfig>;
    superProductFieldsToMonitor: string[];
}

export interface BuyerConfig {
    clientID: string;
    buyerUrl: string;
}