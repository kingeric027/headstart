using Microsoft.Azure.CosmosDB.BulkExecutor.BulkUpdate;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record.PivotTable;
using NPOI.SS.Formula.Functions;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrations.Migrations
{
	public class new_asset_types_8oct2020
	{
		private readonly ICosmosBulkEditor _editor;

		public new_asset_types_8oct2020(ICosmosBulkEditor editor) 
		{
			_editor = editor;
		}

		// This run removes all the existing asset types except images and replaces them with types derived from the ContentType.
		// The old Attachment type is now represented with a specific title, which can be queried on.
		public async Task Run()
		{
			await _editor.RunBulkUpdateAsync<AssetDO>("assets", asset =>
			{
				var updates = new List<UpdateOperation>();

				var typeToken = asset.SelectToken("Type");
				var type = (typeToken.Type != JTokenType.Null) ? typeToken?.ToObject<AssetType>() : null;
				var contentType = asset.SelectToken("Metadata.ContentType")?.ToObject<string>();

				if (type != AssetType.Image)
				{
					var newType = contentType?.ConvertFromContentType() ?? null;
					var updateType = new SetUpdateOperation<AssetType?>("Type", newType);
					updates.Add(updateType);

					if (type?.To<int>() == 2) // old attachment type
					{
						var updateTitle = new SetUpdateOperation<string>("Title", "Product_Attachment");
						updates.Add(updateTitle);
					}
				}
				return updates;
			});
			await _editor.RunBulkUpdateAsync<AssetedResourceDO>("assetedresource", assignment =>
			{
				var themes = assignment.SelectToken("ThemeAssetIDs").ToObject<List<string>>();
				var attachments = assignment.SelectToken("AttachmentAssetIDs").ToObject<List<string>>();
				var structured = assignment.SelectToken("StructuredAssetsIDs").ToObject<List<string>>();

				var array = themes.Concat(attachments).Concat(structured).ToList();
				return new List<UpdateOperation>()
				{
					new SetUpdateOperation<List<string>>("AllOtherAssetIDs", new List<string>())
				};
			});
		}
	}
}
