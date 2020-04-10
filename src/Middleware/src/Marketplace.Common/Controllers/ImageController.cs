using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;
using Marketplace.Models.Attributes;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Orchestration Logs\" represents logs of orchestration activities")]
    [MarketplaceSection.Orchestration(ListOrder = 3)]
    [Route("content/images")]
    public class ImageController : BaseController
    {
        private readonly IImageCommand _command;
        public ImageController(AppSettings settings, IImageCommand command) : base(settings)
        {
            _command = command;
        }

        [DocName("LIST Images")]
        [HttpGet]
        public async Task<ListPage<Image>> List(ListArgs<Image> marketplaceListArgs)
        {
            return await _command.List(marketplaceListArgs);
        }
        [DocName("GET Image")]
        [HttpGet, Route("{id}")]
        public async Task<Image> Get(string id)
        {
            return await _command.Get(id);
        }
        [DocName("POST Image")]
        [HttpPost]
        public async Task<Image> Post([FromBody] Image img)
        {
            return await _command.Create(img);
        }
        [DocName("DELETE Image")]
        [HttpDelete, Route("{id}")]
        public async Task Delete(string id)
        {
            await _command.Delete(id);
        }
    }
}
