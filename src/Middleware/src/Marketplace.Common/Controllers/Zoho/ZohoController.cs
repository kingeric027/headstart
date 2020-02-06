using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Services.Zoho;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Common.Controllers.Zoho
{
    [Route("zoho")]
    public class ZohoController : BaseController
    {
        private readonly IZohoClient _zoho;
        public ZohoController(AppSettings settings, IZohoClient zoho) : base(settings)
        {
            _zoho = zoho;
        }

        [HttpPost, Route("ping")]
        public async Task<dynamic> Ping()
        {
            //var init = _zoho.Init("90f15d12b441726b50524c79acbe8e12", "708539139");
            var org = await _zoho
                .Init(Settings.ZohoSettings.AuthToken, Settings.ZohoSettings.OrgID)
                .Ping(Settings.ZohoSettings.OrgID);
            return org;
        }
    }
}
