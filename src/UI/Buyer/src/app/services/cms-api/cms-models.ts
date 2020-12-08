export interface JDocument {
    ID?: string
    Doc?: any
    readonly SchemaSpecUrl?: string
    readonly History?: History
}

export interface AssetUpload {
    ID?: string
    Title?: string
    Active?: boolean
    File?: File
    Url?: string
    Tags?: string[]
    FileName?: string
  }

export interface AssetMetadata {
    IsUrlOverridden?: boolean
    ContentType?: string
    SizeBytes?: number
    ImageHeight?: number
    ImageWidth?: number
    ImageVerticalResolution?: number
    ImageHorizontalResolution?: number
}

export interface Asset {
    ID?: string
    Title?: string
    Active?: boolean
    Url?: string
    Type?: 'Image' | 'Text' | 'Audio' | 'Video' | 'Presentation' | 'SpreadSheet' | 'PDF' | 'Compressed' | 'Code' | 'JSON' | 'Markup' | 'Unknown'
    Tags?: string[]
    FileName?: string
    readonly Metadata?: AssetMetadata
    readonly History?: History
}

export interface History {
    DateCreated?: string
    CreatedByUserID?: string
    DateUpdated?: string
    UpdatedByUserID?: string
}

export interface DocSchema {
    ID?: string
    RestrictedAssignmentTypes?: 'Catalogs' | 'Categories' | 'Products' | 'PriceSchedules' | 'ProductFacets' | 'Specs' | 'SecurityProfiles' | 'PasswordResets' | 'OpenIdConnects' | 'ImpersonationConfigs' | 'Buyers' | 'Users' | 'UserGroups' | 'Addresses' | 'CostCenters' | 'CreditCards' | 'SpendingAccounts' | 'ApprovalRules' | 'Suppliers' | 'SupplierUsers' | 'SupplierUserGroups' | 'SupplierAddresses' | 'Promotions' | 'AdminUsers' | 'AdminAddresses' | 'AdminUserGroups' | 'MessageSenders' | 'Webhooks' | 'ApiClients' | 'Incrementors' | 'IntegrationEvents' | 'XpIndices'
    Schema?: any
    readonly History?: History
}

export interface DocumentAssignment {
    DocumentID?: string
    ResourceID?: string
    ResourceType?: 'Catalogs' | 'Categories' | 'Products' | 'PriceSchedules' | 'ProductFacets' | 'Specs' | 'SecurityProfiles' | 'PasswordResets' | 'OpenIdConnects' | 'ImpersonationConfigs' | 'Buyers' | 'Users' | 'UserGroups' | 'Addresses' | 'CostCenters' | 'CreditCards' | 'SpendingAccounts' | 'ApprovalRules' | 'Suppliers' | 'SupplierUsers' | 'SupplierUserGroups' | 'SupplierAddresses' | 'Promotions' | 'AdminUsers' | 'AdminAddresses' | 'AdminUserGroups' | 'MessageSenders' | 'Webhooks' | 'ApiClients' | 'Incrementors' | 'IntegrationEvents' | 'XpIndices'
    ParentResourceID?: string
    ParentResourceType?: 'Catalogs' | 'Buyers' | 'Suppliers'
}

export interface AssetAssignment {
    AssetID?: string
    ResourceID?: string
    ResourceType?: 'Catalogs' | 'Categories' | 'Products' | 'PriceSchedules' | 'ProductFacets' | 'Specs' | 'SecurityProfiles' | 'PasswordResets' | 'OpenIdConnects' | 'ImpersonationConfigs' | 'Buyers' | 'Users' | 'UserGroups' | 'Addresses' | 'CostCenters' | 'CreditCards' | 'SpendingAccounts' | 'ApprovalRules' | 'Suppliers' | 'SupplierUsers' | 'SupplierUserGroups' | 'SupplierAddresses' | 'Promotions' | 'AdminUsers' | 'AdminAddresses' | 'AdminUserGroups' | 'MessageSenders' | 'Webhooks' | 'ApiClients' | 'Incrementors' | 'IntegrationEvents' | 'XpIndices'
    ParentResourceID?: string
    ParentResourceType?: 'Catalogs' | 'Buyers' | 'Suppliers'
}

