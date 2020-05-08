using Marketplace.Helpers.Attributes;

namespace Marketplace.Models.Models.Marketplace.Extended
{
	[SwaggerModel]
    public class Currency
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Symbol { get; set; }
        //TODO: Add support for flag, if we can get that added to 
        // the OC-Middleware exchangerates route
    }
}
