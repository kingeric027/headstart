using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ordercloud.integrations.cms.Models
{
	public class Document
	{
		[Required]
		public JObject Doc { get; set; }
	}
}
