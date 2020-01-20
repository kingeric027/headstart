using Marketplace.Common.Helpers;
using Marketplace.Common.Services;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{

	public interface ISendgridCommand
	{
		Task SendSupplierEmails(string orderID);
	}
	public class SendGridCommand : ISendgridCommand
    {
		private readonly ISendgridService _sendgridService;
		private readonly IOrderCloudClient _oc;

		public SendGridCommand(AppSettings settings, ISendgridService sendgridService)
		{
			_sendgridService = sendgridService;
			_oc = OcFactory.GetSEBAdmin();

		}
		public async Task SendSupplierEmails(string orderID)
		{
			var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);
			lineItems.Items
				.Select(item => item.SupplierID)
					.Distinct()
					.ToList()
					.ForEach(async supplier =>
					{
						Supplier supplierInfo = await _oc.Suppliers.GetAsync(supplier);
						await _sendgridService.SendSingleEmail("noreply@four51.com", supplierInfo.xp.Contacts[0].Email, "Order Confirmation", "<h1>this is a test email for order submit</h1>");
					});
		}
	}
}
