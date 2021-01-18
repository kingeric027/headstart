using System.Threading.Tasks;
using Headstart.Common.Commands;
using Headstart.Common.Mappers;
using Headstart.Common.Models;
using Headstart.Models.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Headstart.Common.Controllers
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
