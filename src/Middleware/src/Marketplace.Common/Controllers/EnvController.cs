﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Headstart.Common.Controllers
{
    [Route("env")]
    public class EnvController : BaseController
    {
        public EnvController(AppSettings settings) : base(settings)
        {

        }

        [HttpGet]
        public object Get()
        {
            return new { 
                Environment = Settings.EnvironmentSettings.Environment.ToString(),
                BuildNumber = Settings.EnvironmentSettings.BuildNumber, // set during deploy
                Commit = Settings.EnvironmentSettings.Commit, // set during deploy
                CosmosDatabase = Settings.CosmosSettings.DatabaseName
            };
        }
    }
}
