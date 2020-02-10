using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Models;
using Marketplace.Common.Commands;
using Marketplace.Common.Helpers;
using Marketplace.Helpers.Models;
using Marketplace.Helpers;

namespace Marketplace.Common.Controllers
{
    public class EnvironmentSeedController : BaseController
    {
        private readonly IEnvironmentSeedCommand _command;

        public EnvironmentSeedController(AppSettings settings, IEnvironmentSeedCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpPost, Route("seed"), DevCenterUserAuth()]
        public async Task Seed([FromBody] EnvironmentSeed suppliers)
        {
            await _command.Seed(suppliers, this.VerifiedUserContext);
        }
    }
}
