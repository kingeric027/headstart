using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Commands;
using Marketplace.Common.Controllers;
using Marketplace.Common.Services.AnytimeDashboard.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Common.Controllerss
{
	[MarketplaceSection.Marketplace(ListOrder = 1)]
	[Route("api/af-webhooks")]
	public class AnytimeWebhookController : BaseController
	{
		private readonly IAnytimeWebhookCommand _command;

		// https://api.anytimefitness.com/Help/Webhooks
		public AnytimeWebhookController(IAnytimeWebhookCommand command, AppSettings settings) : base(settings)
		{
			_command = command;
		}

		[HttpPost]
		public async Task HandleAnytimeEventNotification([FromBody] List<Notification> notifications)
		{
			await _command.HandleAnytimeEventNotifications(notifications);
		}
	}
}
