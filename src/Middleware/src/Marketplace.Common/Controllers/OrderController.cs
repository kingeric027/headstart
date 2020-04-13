using Marketplace.Common.Commands;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Helpers.Attributes;
using Marketplace.Models.Attributes;
using Marketplace.Models.Misc;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Marketplace Orders\" for handling order commands in Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 2)]
    [Route("order")]
    public class OrderController : BaseController
    {
        
        private readonly IOrderCommand _command;
        public OrderController(IOrderCommand command, AppSettings settings) : base(settings)
        {
            _command = command;
        }

        [DocName("POST Acknowledge Quote Order")]
        // todo update auth
        [HttpPost]
        [Route("acknowledgequote/{orderID}")]
        public async Task<Order> AcknowledgeQuoteOrder(string orderID)
        {
            return await _command.AcknowledgeQuoteOrder(orderID);
        } 
    }
}
