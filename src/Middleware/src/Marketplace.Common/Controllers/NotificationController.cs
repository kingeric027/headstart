using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Headstart.Common.Commands;
using Headstart.Common.Commands.Crud;
using Headstart.Common.Models.Marketplace;
using Headstart.Common.Services.CMS.Models;
using Headstart.Models;
using Headstart.Models.Attributes;
using Headstart.Models.Misc;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Headstart.Common.Controllers
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
		public async Task<SuperHSProduct> CreateModifiedMonitoredSuperProductNotification([FromBody] MonitoredProductFieldModifiedNotification notification)
		{
			return await _command.CreateModifiedMonitoredSuperProductNotification(notification, VerifiedUserContext);
		}

		[DocName("PUT Monitored Product Field Modified")]
		[HttpPut, Route("monitored-product-field-modified/{documentID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<SuperHSProduct> UpdateMonitoredSuperProductNotificationStatus([FromBody] Document<MonitoredProductFieldModifiedNotification> document)
		{
			return await _command.UpdateMonitoredSuperProductNotificationStatus(document, document.Doc.Supplier.ID, document.Doc.Product.ID, VerifiedUserContext);
		}

		[DocName("GET Monitored Product Field Modified")]
		[HttpPost, Route("monitored-product-notification"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<ListPage<Document<MonitoredProductFieldModifiedNotification>>> ReadMonitoredSuperProductNotificationStatus([FromBody] SuperHSProduct product)
		{
			return await _command.ReadMonitoredSuperProductNotificationList(product, VerifiedUserContext);
		}
	}
}
