import { ListArgs } from '@ordercloud/headstart-sdk';
import { ListPage, RequiredDeep } from 'ordercloud-javascript-sdk';
import { Asset, AssetAssignment, AssetUpload, DocSchema, DocumentAssignment, JDocument } from './cms-models';
import { httpClient, TokenService } from './cms-utils';

class Schemas {
    private impersonating:boolean = false;

    /**
    * @ignore
    * not part of public api, don't include in generated docs
    */
    constructor() {
        this.List = this.List.bind(this);
        this.Create = this.Create.bind(this);
        this.Get = this.Get.bind(this);
        this.Save = this.Save.bind(this);
        this.Delete = this.Delete.bind(this);
    }

   /**
    * @param options.search Word or phrase to search for.
    * @param options.searchOn Comma-delimited list of fields to search on.
    * @param options.sortBy Comma-delimited list of fields to sort by.
    * @param options.page Page of results to return. Default: 1
    * @param options.pageSize Number of results to return per page. Default: 20, max: 100.
    * @param options.filters An object whose keys match the model, and the values are the values to filter by
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async List( options: ListArgs<DocSchema> = {}, accessToken?: string ): Promise<RequiredDeep<ListPage<DocSchema>>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/schemas`, { params: { ...options,  filters: options.filters, accessToken, impersonating } } );
    }

   /**
    * @param docSchema Required fields: Schema
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Create(docSchema: DocSchema, accessToken?: string ): Promise<RequiredDeep<DocSchema>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.post(`/schemas`, docSchema, { params: {  accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Get(schemaID: string,  accessToken?: string ): Promise<RequiredDeep<DocSchema>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/schemas/${schemaID}`, { params: {  accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param docSchema Required fields: Schema
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Save(schemaID: string, docSchema: DocSchema, accessToken?: string ): Promise<RequiredDeep<DocSchema>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.put(`/schemas/${schemaID}`, docSchema, { params: {  accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Delete(schemaID: string,  accessToken?: string ): Promise<void> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.delete(`/schemas/${schemaID}`, { params: {  accessToken, impersonating } } );
    }

    /**
     * @description 
     * enables impersonation by calling the subsequent method with the stored impersonation token
     * 
     * @example
     * Schemas.As().List() // lists Schemas using the impersonated users' token
     */
    public As(): this {
        this.impersonating = true;
        return this;
    }
}

class Assets {
    private impersonating:boolean = false;

    /**
    * @ignore
    * not part of public api, don't include in generated docs
    */
    constructor() {
        this.List = this.List.bind(this);
        this.Get = this.Get.bind(this);
        this.Upload = this.Upload.bind(this);
        this.Save = this.Save.bind(this);
        this.Delete = this.Delete.bind(this);
        this.ListAssetsOnChild = this.ListAssetsOnChild.bind(this);
        this.GetThumbnailOnChild = this.GetThumbnailOnChild.bind(this);
        this.GetThumbnail = this.GetThumbnail.bind(this);
        this.ListAssets = this.ListAssets.bind(this);
        this.SaveAssetAssignment = this.SaveAssetAssignment.bind(this);
        this.DeleteAssetAssignment = this.DeleteAssetAssignment.bind(this);
        this.MoveImageAssignment = this.MoveImageAssignment.bind(this);
    }

   /**
    * @param options.search Word or phrase to search for.
    * @param options.searchOn Comma-delimited list of fields to search on.
    * @param options.sortBy Comma-delimited list of fields to sort by.
    * @param options.page Page of results to return. Default: 1
    * @param options.pageSize Number of results to return per page. Default: 20, max: 100.
    * @param options.filters An object whose keys match the model, and the values are the values to filter by
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async List( options: ListArgs<Asset> = {}, accessToken?: string ): Promise<RequiredDeep<ListPage<Asset>>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/assets`, { params: { ...options,  filters: options.filters, accessToken, impersonating } } );
    }

   /**
    * @param assetID ID of the asset.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Get(assetID: string,  accessToken?: string ): Promise<RequiredDeep<Asset>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/assets/${assetID}`, { params: {  accessToken, impersonating } } );
    }

    /**
    * @param asset Upload.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Upload(asset: AssetUpload, accessToken?: string): Promise<RequiredDeep<Asset>> {
        const form = new FormData()
        for (const prop in asset) {
          if (asset.hasOwnProperty(prop)) {
            form.append(prop, asset[prop])
          }
        }
        return await httpClient.post(`/assets`, form, { params: { accessToken } })
      }

   /**
    * @param assetID ID of the asset.
    * @param asset 
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Save(assetID: string, asset: Asset, accessToken?: string ): Promise<RequiredDeep<Asset>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.put(`/assets/${assetID}`, asset, { params: {  accessToken, impersonating } } );
    }

   /**
    * @param assetID ID of the asset.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Delete(assetID: string,  accessToken?: string ): Promise<void> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.delete(`/assets/${assetID}`, { params: {  accessToken, impersonating } } );
    }

   /**
    * @param parentType Parent type of the asset. Possible values: Catalogs, Buyers, Suppliers.
    * @param parentID ID of the parent.
    * @param type Type of the asset. Possible values: Catalogs, Categories, Products, PriceSchedules, ProductFacets, Specs, SecurityProfiles, PasswordResets, OpenIdConnects, ImpersonationConfigs, Buyers, Users, UserGroups, Addresses, CostCenters, CreditCards, SpendingAccounts, ApprovalRules, Suppliers, SupplierUsers, SupplierUserGroups, SupplierAddresses, Promotions, AdminUsers, AdminAddresses, AdminUserGroups, MessageSenders, Webhooks, ApiClients, Incrementors, IntegrationEvents, XpIndices.
    * @param ID ID of the asset.
    * @param options.args Args of the asset.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async ListAssetsOnChild(parentType: 'Catalogs' | 'Buyers' | 'Suppliers', parentID: string, type: 'Catalogs' | 'Categories' | 'Products' | 'PriceSchedules' | 'ProductFacets' | 'Specs' | 'SecurityProfiles' | 'PasswordResets' | 'OpenIdConnects' | 'ImpersonationConfigs' | 'Buyers' | 'Users' | 'UserGroups' | 'Addresses' | 'CostCenters' | 'CreditCards' | 'SpendingAccounts' | 'ApprovalRules' | 'Suppliers' | 'SupplierUsers' | 'SupplierUserGroups' | 'SupplierAddresses' | 'Promotions' | 'AdminUsers' | 'AdminAddresses' | 'AdminUserGroups' | 'MessageSenders' | 'Webhooks' | 'ApiClients' | 'Incrementors' | 'IntegrationEvents' | 'XpIndices', ID: string,  options: ListArgs<Asset> = {}, accessToken?: string ): Promise<RequiredDeep<ListPage<Asset>>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/assets/${parentType}/${parentID}/${type}/${ID}`, { params: { ...options,  accessToken, impersonating } } );
    }

   /**
    * @param sellerID ID of the seller.
    * @param parentType Parent type of the asset. Possible values: Catalogs, Buyers, Suppliers.
    * @param parentID ID of the parent.
    * @param type Type of the asset. Possible values: Catalogs, Categories, Products, PriceSchedules, ProductFacets, Specs, SecurityProfiles, PasswordResets, OpenIdConnects, ImpersonationConfigs, Buyers, Users, UserGroups, Addresses, CostCenters, CreditCards, SpendingAccounts, ApprovalRules, Suppliers, SupplierUsers, SupplierUserGroups, SupplierAddresses, Promotions, AdminUsers, AdminAddresses, AdminUserGroups, MessageSenders, Webhooks, ApiClients, Incrementors, IntegrationEvents, XpIndices.
    * @param ID ID of the asset.
    * @param options.size Size of the asset. Possible values: S, M.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async GetThumbnailOnChild(sellerID: string, parentType: 'Catalogs' | 'Buyers' | 'Suppliers', parentID: string, type: 'Catalogs' | 'Categories' | 'Products' | 'PriceSchedules' | 'ProductFacets' | 'Specs' | 'SecurityProfiles' | 'PasswordResets' | 'OpenIdConnects' | 'ImpersonationConfigs' | 'Buyers' | 'Users' | 'UserGroups' | 'Addresses' | 'CostCenters' | 'CreditCards' | 'SpendingAccounts' | 'ApprovalRules' | 'Suppliers' | 'SupplierUsers' | 'SupplierUserGroups' | 'SupplierAddresses' | 'Promotions' | 'AdminUsers' | 'AdminAddresses' | 'AdminUserGroups' | 'MessageSenders' | 'Webhooks' | 'ApiClients' | 'Incrementors' | 'IntegrationEvents' | 'XpIndices', ID: string,  size: 'S' | 'M', accessToken?: string ): Promise<void> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/assets/${sellerID}/${parentType}/${parentID}/${type}/${ID}/thumbnail`, { params: { size,  accessToken, impersonating } } );
    }

   /**
    * @param sellerID ID of the seller.
    * @param type Type of the asset. Possible values: Catalogs, Categories, Products, PriceSchedules, ProductFacets, Specs, SecurityProfiles, PasswordResets, OpenIdConnects, ImpersonationConfigs, Buyers, Users, UserGroups, Addresses, CostCenters, CreditCards, SpendingAccounts, ApprovalRules, Suppliers, SupplierUsers, SupplierUserGroups, SupplierAddresses, Promotions, AdminUsers, AdminAddresses, AdminUserGroups, MessageSenders, Webhooks, ApiClients, Incrementors, IntegrationEvents, XpIndices.
    * @param ID ID of the asset.
    * @param options.size Size of the asset. Possible values: S, M.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async GetThumbnail(sellerID: string, type: 'Catalogs' | 'Categories' | 'Products' | 'PriceSchedules' | 'ProductFacets' | 'Specs' | 'SecurityProfiles' | 'PasswordResets' | 'OpenIdConnects' | 'ImpersonationConfigs' | 'Buyers' | 'Users' | 'UserGroups' | 'Addresses' | 'CostCenters' | 'CreditCards' | 'SpendingAccounts' | 'ApprovalRules' | 'Suppliers' | 'SupplierUsers' | 'SupplierUserGroups' | 'SupplierAddresses' | 'Promotions' | 'AdminUsers' | 'AdminAddresses' | 'AdminUserGroups' | 'MessageSenders' | 'Webhooks' | 'ApiClients' | 'Incrementors' | 'IntegrationEvents' | 'XpIndices', ID: string,  size: 'S' | 'M', accessToken?: string ): Promise<void> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/assets/${sellerID}/${type}/${ID}/thumbnail`, { params: { size,  accessToken, impersonating } } );
    }

   /**
    * @param type Type of the asset. Possible values: Catalogs, Categories, Products, PriceSchedules, ProductFacets, Specs, SecurityProfiles, PasswordResets, OpenIdConnects, ImpersonationConfigs, Buyers, Users, UserGroups, Addresses, CostCenters, CreditCards, SpendingAccounts, ApprovalRules, Suppliers, SupplierUsers, SupplierUserGroups, SupplierAddresses, Promotions, AdminUsers, AdminAddresses, AdminUserGroups, MessageSenders, Webhooks, ApiClients, Incrementors, IntegrationEvents, XpIndices.
    * @param ID ID of the asset.
    * @param options.args Args of the asset.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async ListAssets(type: 'Catalogs' | 'Categories' | 'Products' | 'PriceSchedules' | 'ProductFacets' | 'Specs' | 'SecurityProfiles' | 'PasswordResets' | 'OpenIdConnects' | 'ImpersonationConfigs' | 'Buyers' | 'Users' | 'UserGroups' | 'Addresses' | 'CostCenters' | 'CreditCards' | 'SpendingAccounts' | 'ApprovalRules' | 'Suppliers' | 'SupplierUsers' | 'SupplierUserGroups' | 'SupplierAddresses' | 'Promotions' | 'AdminUsers' | 'AdminAddresses' | 'AdminUserGroups' | 'MessageSenders' | 'Webhooks' | 'ApiClients' | 'Incrementors' | 'IntegrationEvents' | 'XpIndices', ID: string,  options: ListArgs<Asset> = {}, accessToken?: string ): Promise<RequiredDeep<ListPage<Asset>>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/assets/${type}/${ID}`, { params: { ...options,  accessToken, impersonating } } );
    }

   /**
    * @param assetAssignment 
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async SaveAssetAssignment(assetAssignment: AssetAssignment, accessToken?: string ): Promise<void> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.post(`/assets/assignments`, assetAssignment, { params: {  accessToken, impersonating } } );
    }

   /**
    * @param options.AssetID 
    * @param options.ResourceID 
    * @param options.ResourceType 
    * @param options.ParentResourceID 
    * @param options.ParentResourceType 
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async DeleteAssetAssignment( AssetID: string, ResourceID: string, ResourceType: string, ParentResourceID: string, ParentResourceType: string, accessToken?: string ): Promise<void> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.delete(`/assets/assignments`, { params: { AssetID, ResourceID, ResourceType, ParentResourceID, ParentResourceType,  accessToken, impersonating } } );
    }

   /**
    * @param newPosition New position of the asset assignment.
    * @param assetAssignment 
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async MoveImageAssignment(newPosition: number, assetAssignment: AssetAssignment, accessToken?: string ): Promise<void> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.post(`/assets/assignments/moveto/${newPosition}`, assetAssignment, { params: {  accessToken, impersonating } } );
    }

    /**
     * @description 
     * enables impersonation by calling the subsequent method with the stored impersonation token
     * 
     * @example
     * Assets.As().List() // lists Assets using the impersonated users' token
     */
    public As(): this {
        this.impersonating = true;
        return this;
    }
}

class Documents {
    private impersonating:boolean = false;

    /**
    * @ignore
    * not part of public api, don't include in generated docs
    */
    constructor() {
        this.List = this.List.bind(this);
        this.Create = this.Create.bind(this);
        this.Get = this.Get.bind(this);
        this.Save = this.Save.bind(this);
        this.Delete = this.Delete.bind(this);
        this.ListDocumentsOnChild = this.ListDocumentsOnChild.bind(this);
        this.ListDocuments = this.ListDocuments.bind(this);
        this.ListAssignments = this.ListAssignments.bind(this);
        this.SaveAssignment = this.SaveAssignment.bind(this);
        this.DeleteAssignment = this.DeleteAssignment.bind(this);
    }

   /**
    * @param schemaID ID of the schema.
    * @param options.search Word or phrase to search for.
    * @param options.searchOn Comma-delimited list of fields to search on.
    * @param options.sortBy Comma-delimited list of fields to sort by.
    * @param options.page Page of results to return. Default: 1
    * @param options.pageSize Number of results to return per page. Default: 20, max: 100.
    * @param options.filters An object whose keys match the model, and the values are the values to filter by
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async List(schemaID: string,  options: ListArgs<JDocument> = {}, accessToken?: string ): Promise<RequiredDeep<ListPage<JDocument>>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/schemas/${schemaID}/documents`, { params: { ...options,  filters: options.filters, accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param jDocument 
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Create(schemaID: string, jDocument: JDocument, accessToken?: string ): Promise<RequiredDeep<JDocument>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.post(`/schemas/${schemaID}/documents`, jDocument, { params: {  accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param documentID ID of the document.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Get(schemaID: string, documentID: string,  accessToken?: string ): Promise<RequiredDeep<JDocument>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/schemas/${schemaID}/documents/${documentID}`, { params: {  accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param documentID ID of the document.
    * @param jDocument 
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Save(schemaID: string, documentID: string, jDocument: JDocument, accessToken?: string ): Promise<RequiredDeep<JDocument>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.put(`/schemas/${schemaID}/documents/${documentID}`, jDocument, { params: {  accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param documentID ID of the document.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async Delete(schemaID: string, documentID: string,  accessToken?: string ): Promise<void> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.delete(`/schemas/${schemaID}/documents/${documentID}`, { params: {  accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param parentType Parent type of the j document. Possible values: Catalogs, Buyers, Suppliers.
    * @param parentID ID of the parent.
    * @param type Type of the j document. Possible values: Catalogs, Categories, Products, PriceSchedules, ProductFacets, Specs, SecurityProfiles, PasswordResets, OpenIdConnects, ImpersonationConfigs, Buyers, Users, UserGroups, Addresses, CostCenters, CreditCards, SpendingAccounts, ApprovalRules, Suppliers, SupplierUsers, SupplierUserGroups, SupplierAddresses, Promotions, AdminUsers, AdminAddresses, AdminUserGroups, MessageSenders, Webhooks, ApiClients, Incrementors, IntegrationEvents, XpIndices.
    * @param ID ID of the j document.
    * @param options.args Args of the j document.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async ListDocumentsOnChild(schemaID: string, parentType: 'Catalogs' | 'Buyers' | 'Suppliers', parentID: string, type: 'Catalogs' | 'Categories' | 'Products' | 'PriceSchedules' | 'ProductFacets' | 'Specs' | 'SecurityProfiles' | 'PasswordResets' | 'OpenIdConnects' | 'ImpersonationConfigs' | 'Buyers' | 'Users' | 'UserGroups' | 'Addresses' | 'CostCenters' | 'CreditCards' | 'SpendingAccounts' | 'ApprovalRules' | 'Suppliers' | 'SupplierUsers' | 'SupplierUserGroups' | 'SupplierAddresses' | 'Promotions' | 'AdminUsers' | 'AdminAddresses' | 'AdminUserGroups' | 'MessageSenders' | 'Webhooks' | 'ApiClients' | 'Incrementors' | 'IntegrationEvents' | 'XpIndices', ID: string,  options: ListArgs<JDocument> = {}, accessToken?: string ): Promise<RequiredDeep<ListPage<JDocument>>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/schemas/${schemaID}/documents/${parentType}/${parentID}/${type}/${ID}`, { params: { ...options,  accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param type Type of the j document. Possible values: Catalogs, Categories, Products, PriceSchedules, ProductFacets, Specs, SecurityProfiles, PasswordResets, OpenIdConnects, ImpersonationConfigs, Buyers, Users, UserGroups, Addresses, CostCenters, CreditCards, SpendingAccounts, ApprovalRules, Suppliers, SupplierUsers, SupplierUserGroups, SupplierAddresses, Promotions, AdminUsers, AdminAddresses, AdminUserGroups, MessageSenders, Webhooks, ApiClients, Incrementors, IntegrationEvents, XpIndices.
    * @param ID ID of the j document.
    * @param options.args Args of the j document.
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async ListDocuments(schemaID: string, type: 'Catalogs' | 'Categories' | 'Products' | 'PriceSchedules' | 'ProductFacets' | 'Specs' | 'SecurityProfiles' | 'PasswordResets' | 'OpenIdConnects' | 'ImpersonationConfigs' | 'Buyers' | 'Users' | 'UserGroups' | 'Addresses' | 'CostCenters' | 'CreditCards' | 'SpendingAccounts' | 'ApprovalRules' | 'Suppliers' | 'SupplierUsers' | 'SupplierUserGroups' | 'SupplierAddresses' | 'Promotions' | 'AdminUsers' | 'AdminAddresses' | 'AdminUserGroups' | 'MessageSenders' | 'Webhooks' | 'ApiClients' | 'Incrementors' | 'IntegrationEvents' | 'XpIndices', ID: string,  options: ListArgs<JDocument> = {}, accessToken?: string ): Promise<RequiredDeep<ListPage<JDocument>>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/schemas/${schemaID}/documents/${type}/${ID}`, { params: { ...options,  accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param options.search Word or phrase to search for.
    * @param options.searchOn Comma-delimited list of fields to search on.
    * @param options.sortBy Comma-delimited list of fields to sort by.
    * @param options.page Page of results to return. Default: 1
    * @param options.pageSize Number of results to return per page. Default: 20, max: 100.
    * @param options.filters An object whose keys match the model, and the values are the values to filter by
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async ListAssignments(schemaID: string,  options: ListArgs<DocumentAssignment> = {}, accessToken?: string ): Promise<RequiredDeep<ListPage<DocumentAssignment>>> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.get(`/schemas/${schemaID}/documents/assignments`, { params: { ...options,  filters: options.filters, accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param documentAssignment 
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async SaveAssignment(schemaID: string, documentAssignment: DocumentAssignment, accessToken?: string ): Promise<void> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.post(`/schemas/${schemaID}/documents/assignments`, documentAssignment, { params: {  accessToken, impersonating } } );
    }

   /**
    * @param schemaID ID of the schema.
    * @param options.DocumentID 
    * @param options.ResourceID 
    * @param options.ResourceType 
    * @param options.ParentResourceID 
    * @param options.ParentResourceType 
    * @param accessToken Provide an alternative token to the one stored in the sdk instance (useful for impersonation).
    */
    public async DeleteAssignment(schemaID: string,  DocumentID: string, ResourceID: string, ResourceType: string, ParentResourceID: string, ParentResourceType: string, accessToken?: string ): Promise<void> {
        const impersonating = this.impersonating;
        this.impersonating = false;
        return await httpClient.delete(`/schemas/${schemaID}/documents/assignments`, { params: { DocumentID, ResourceID, ResourceType, ParentResourceID, ParentResourceType,  accessToken, impersonating } } );
    }

    /**
     * @description 
     * enables impersonation by calling the subsequent method with the stored impersonation token
     * 
     * @example
     * Documents.As().List() // lists Documents using the impersonated users' token
     */
    public As(): this {
        this.impersonating = true;
        return this;
    }
}

export const ContentManagementClient = {
    Assets : new Assets(),
    Documents : new Documents(),
    Schemas : new Schemas(),
    Tokens: new TokenService()
}