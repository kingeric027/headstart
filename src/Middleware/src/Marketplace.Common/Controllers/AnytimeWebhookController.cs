using System.Collections.Generic;
using System.Threading.Tasks;
using Headstart.Common.Commands;
using Headstart.Common.Controllers;
using Headstart.Common.Services.AnytimeDashboard.Models;
using Headstart.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;

namespace Headstart.Common.Controllerss
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
		[DocName("Anytime Webhook")]
		public async Task HandleAnytimeEventNotification([FromBody] ListNotification notifications)
		{
			await _command.HandleAnytimeEventNotifications(notifications);
		}

	}

	// need this temporary model to mitigate shortcomings with sdk generation
	[SwaggerModel]
	public class ListNotification : List<Notification>
    {

    }
}
