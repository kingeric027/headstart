using System;
using System.Threading.Tasks;
using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Marketplace.Models.Misc;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
	[DocComments("Notifications")]
	[Route("notifications")]
	public class NotificationController : BaseController
	{

		private readonly INotificationCommand _command;
		public NotificationController(AppSettings settings, INotificationCommand command) : base(settings)
		{
			_command = command;
		}

		[DocName("POST Monitored Product Field Modified")]
		[HttpPost, Route("monitored-product-field-modified"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<SuperMarketplaceProduct> CreateModifiedMonitoredSuperProductNotification([FromBody] MonitoredProductFieldModifiedNotification notification)
		{
			return await _command.CreateModifiedMonitoredSuperProductNotification(notification, VerifiedUserContext);
		}

		[DocName("PUT Monitored Product Field Modified")]
		[HttpPut, Route("monitored-product-field-modified/{documentID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<SuperMarketplaceProduct> UpdateMonitoredSuperProductNotificationStatus([FromBody] MonitoredProductFieldModifiedNotificationDocument document)
		{
			return await _command.UpdateMonitoredSuperProductNotificationStatus(document, document.Doc.Supplier.ID, document.Doc.Product.ID, VerifiedUserContext);
		}

	}
}
