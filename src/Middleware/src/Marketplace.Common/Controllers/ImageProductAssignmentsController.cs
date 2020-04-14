using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;
using Marketplace.Models.Attributes;
using OrderCloud.SDK;
using Marketplace.Models;

namespace Marketplace.Common.Controllers
{
    [Route("content/images")]
    public class ImageProductAssignmentsController : BaseController
    {
        private readonly IImageProductAssignmentCommand _command;
        public ImageProductAssignmentsController(AppSettings settings, IImageProductAssignmentCommand command) : base(settings)
        {
            _command = command;
        }

        [DocName("LIST Image Product Assignments")]
        [HttpGet, Route("productassignments")]
        public async Task<ListPage<ImageProductAssignment>> List(ListArgs<ImageProductAssignment> marketplaceListArgs)
        {
            return await _command.List(marketplaceListArgs);
        }
        [DocName("POST Image Product Assignment")]
        [HttpPost, Route("productassignments"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public async Task<ImageProductAssignment> Post([FromBody] ImageProductAssignment pia)
        {
            return await _command.Create(pia, VerifiedUserContext);
        }
        [DocName("DELETE Product Image Assignment")]
        [HttpDelete, Route("{ImageID}/productassignments/{ProductID}")]
        public async Task Delete(string ImageID, string ProductID)
        {
            await _command.Delete(ImageID, ProductID);
        }
    }
}
