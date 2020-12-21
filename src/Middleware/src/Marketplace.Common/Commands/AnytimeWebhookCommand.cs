using Marketplace.Common.Mappers;
using Marketplace.Common.Services.AnytimeDashboard.Models;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
	public interface IAnytimeWebhookCommand
	{
		Task HandleAnytimeEventNotifications(List<Notification> notifications);
	}

	public class AnytimeWebhookCommand : IAnytimeWebhookCommand
	{
		private readonly OrdercloudDataConfig _config;
		private readonly IOrderCloudClient _oc;
		private readonly IMarketplaceBuyerLocationCommand _locationCommand;

		public AnytimeWebhookCommand(AppSettings settings, IOrderCloudClient oc, IMarketplaceBuyerLocationCommand locationCommand) {
			_config = settings.OrderCloudSettings.DataConfig;
			_oc = oc;
			_locationCommand = locationCommand;
		}

		public async Task HandleAnytimeEventNotifications(List<Notification> notifications)
		{
			var handlers = notifications.Select(ChooseHandlerFunction);
			await Task.WhenAll(handlers);
		}

		private Task ChooseHandlerFunction(Notification notification)
		{
			if (notification.Channel == "Club" && notification.Action == "Update")
			{
				return UpdateClub(notification.GetData<AFClub>());
			} else if (notification.Channel == "Club" && notification.Action == "Delete") 
			{
				return DeleteClub(notification.GetData<AFClub>());
			}
			else
			{
				return Task.FromResult(0); // do nothing
			}
		}

		private async Task UpdateClub(AFClub club)
		{ 
			if (club.isDeleted) // if club is inactivated
			{
				var location = LocationMapper.MapToLocation(_config.AfBuyerID, club);
				try
				{ 
					var address = await _oc.Addresses.GetAsync(_config.AfBuyerID, location.Address.ID); // and if it exists in Orderlcoud 
					await _locationCommand.Delete(_config.AfBuyerID, location.Address.ID, _oc.TokenResponse.AccessToken); // then delete it
				} catch { }  // do nothing
			}
			// For now, do nothing if the club is active. At some point, this could be used to sync upadates. 
		}

		private async Task DeleteClub(AFClub club)
		{
			var location = LocationMapper.MapToLocation(_config.AfBuyerID, club);
			await _locationCommand.Delete(_config.AfBuyerID, location.Address.ID, _oc.TokenResponse.AccessToken);
		}
	}
}
