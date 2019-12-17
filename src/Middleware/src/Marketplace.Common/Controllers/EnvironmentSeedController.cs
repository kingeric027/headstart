﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Common.Commands;

namespace Marketplace.Common.Controllers
{
    public class EnvironmentSeedController : BaseController
    {
        private readonly IEnvironmentSeedCommand _command;

        public EnvironmentSeedController(IAppSettings settings, IEnvironmentSeedCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpPost, Route("seed"), MarketplaceUserAuth()]
        public async Task Seed([FromBody] EnvironmentSeed obj)
        {
            await _command.Seed(obj, this.VerifiedUserContext);
        }
    }
}