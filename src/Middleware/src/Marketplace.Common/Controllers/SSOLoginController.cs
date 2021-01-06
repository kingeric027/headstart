using System.Threading.Tasks;
using Marketplace.Common.Commands;
using Marketplace.Common.Mappers;
using Marketplace.Common.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Common.Controllers
{
	[MarketplaceSection.Marketplace(ListOrder = 1)]
	[Route("api")]
	public class SSOLoginController : BaseController
	{
		private readonly ISSOLoginCommand _command;

		public SSOLoginController(ISSOLoginCommand command, AppSettings settings) : base(settings)
		{
			_command = command;
		}

		[Route("anytime/sso"), HttpGet]
		public void RedirectToAnytimeAuthorize([FromQuery] string path = "")
		{
			var state = Coding.EncodeState(new SSOState() { Path = path });
			var url = _command.BuildAuthorizeUrl(FranchiseEnum.AnytimeFitness, state);
			Response.Redirect(url);
		}

		[Route("anytime/authorize"), HttpGet]
		public async Task RedirectToAnytimeStorefront([FromQuery] string code, [FromQuery] string state)
		{
			var url = await _command.BuildStorefrontUrl(FranchiseEnum.AnytimeFitness, code, state);
			Response.Redirect(url);
		}

		[Route("waxing/sso"), HttpGet]
		public void RedirectToWaxingAuthorize([FromQuery] string path = "")
		{		
			var state = Coding.EncodeState(new SSOState() { Path = path });
			var url = _command.BuildAuthorizeUrl(FranchiseEnum.WaxingTheCity, state);
			Response.Redirect(url);
		}

		[Route("waxing/authorize"), HttpGet]
		public async Task RedirectToWaxingStorefront([FromQuery] string code, [FromQuery] string state)
		{
			var url = await _command.BuildStorefrontUrl(FranchiseEnum.WaxingTheCity, code, state);
			Response.Redirect(url);
		}
	}
}
