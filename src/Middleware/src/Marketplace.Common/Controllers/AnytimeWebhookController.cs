using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Commands;
using Marketplace.Common.Services.AnytimeDashboard.Models;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Common.Controllerss
{
	[Route("api/af-webhooks")]
	public class AnytimeWebhookController : Controller
	{
		private readonly IAnytimeWebhookCommand _command;

		// https://api.anytimefitness.com/Help/Webhooks
		public AnytimeWebhookController(IAnytimeWebhookCommand command)
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
