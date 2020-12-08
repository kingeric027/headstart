using Marketplace.Common.Services.CMS.Models;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Services.CMS
{
	public interface ICMSClient
	{
		IAssetResource Assets { get; }
		IDocumentResource Documents { get; }
		ISchemaResource Schemas { get; }
	}

	public interface IAssetResource
	{
		Task<ListPage<Asset>> List(ListArgs<Asset> args, string token);
		Task<Asset> Get(string assetID, string token);
		Task<Asset> Save(string assetID, Asset asset, string token);
		Task Delete(string assetID, string token);
		Task SaveAssetAssignment(AssetAssignment assignment, string token);
		Task DeleteAssetAssignment(AssetAssignment assignment, string token);
		Task<ListPage<Asset>> ListAssets(ResourceType type, string ID, ListArgsPageOnly args, string token);
		Task<ListPage<Asset>> ListAssetsOnChild(ParentResourceType parentType, string parentID, ResourceType type, string ID, ListArgsPageOnly args, string token);
	}

	public interface IDocumentResource
	{
		Task<ListPage<Document<T>>> List<T>(string schemaID, ListArgs<Document<T>> args, string token);
		Task<Document<T>> Get<T>(string schemaID, string documentID, string token);
		Task<Document<T>> Create<T>(string schemaID, Document<T> document, string token);
		Task<Document<T>> Save<T>(string schemaID, string documentID, Document<T> document, string token);
		Task Delete(string schemaID, string documentID, string token);
		Task<ListPage<DocumentAssignment>> ListAssignments(string schemaID, ListArgs<DocumentAssignment> args, string token);
		Task SaveAssignment(string schemaID, DocumentAssignment assignment, string token);
		Task DeleteAssignment(string schemaID, DocumentAssignment assignment, string token);
		Task<ListPage<Document<T>>> ListDocuments<T>(string schemaID, ResourceType type, string ID, ListArgsPageOnly args, string token);
		Task<ListPage<Document<T>>> ListDocumentsOnChild<T>(string schemaID, ParentResourceType parentType, string parentID, ResourceType type, string ID, ListArgsPageOnly args, string token);
	}

	public interface ISchemaResource
	{
		Task<ListPage<DocSchema>> List(ListArgs<DocSchema> args, string token);
		Task<DocSchema> Get(string schemaID, string token);
		Task<DocSchema> Create(DocSchema schema, string token);
		Task<DocSchema> Save(string schemaID, DocSchema schema, string token);
		Task Delete(string schemaID, string token);
	}
}
