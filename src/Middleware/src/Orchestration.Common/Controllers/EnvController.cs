using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Orchestration.Common.Controllers
{
    [Route("env")]
    public class EnvController : BaseController
    {
        public EnvController(IAppSettings settings) : base(settings)
        {
        }

        [HttpGet]
        public async Task<IAppSettings> Get()
        {
            return await Task.FromResult(Settings);
        }
    }
}
