using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Helpers;
using Marketplace.Models.Misc;
using Marketplace.Models.Models.Misc;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marketplace.Common.Services.Avalara.Mappers
{
	public static class AvalaraMapper
	{
		// See https://sandbox-rest.avatax.com/api/v2/definitions/certificateexemptreasons for all available reasons
		private readonly static string RESALE_EXEMPTION_REASON = "RESALE";

		// Resale Exemption Certificates
		public static CertificateModel Map(TaxCertificate source)
		{
			return new CertificateModel()
			{
				id = int.Parse(source.ID),
				signedDate = source.SignedDate.UtcDateTime,
				expirationDate = source.ExpirationDate.UtcDateTime,
				pdf = source.Base64UrlEncodedPDF,
				exemptPercentage = 100,
				exemptionNumber = source.ExemptionNumber,
				exposureZone = new ExposureZoneModel()
				{
					name = source.ExposureZoneName
				},
				exemptionReason = new ExemptionReasonModel()
				{
					name = RESALE_EXEMPTION_REASON
				}
			};
		}

		public static TaxCertificate Map(CertificateModel source)
		{
			return new TaxCertificate()
			{
				ID = source.id.ToString(),
				SignedDate = source.signedDate,
				ExpirationDate = source.expirationDate,
				ExemptionNumber = source.exemptionNumber,
				ExposureZoneName = source.exposureZone.name
				// Skip pdf intentionally. It has its own download route.
			};
		}

		// Tax Codes for lines on Transactions

		public static ListPage<TaxCode> Map(FetchResult<TaxCodeModel> codes, TaxCodeListArgs args)
		{
			var items = codes.value.Select(code => new TaxCode
			{
				Category = args.CodeCategory,
				Code = code.taxCode,
				Description = code.description
			}).ToList();
			var listPage = new ListPage<TaxCode>
			{
				Items = items,
				Meta = new ListPageMeta
				{
					Page = (int)Math.Ceiling((double)args.Skip / args.Top) + 1,
					PageSize = 100,
					TotalCount = codes.count,
				}
			};
			return listPage;
		}

		public static TaxCodeListArgs Map(ListArgs<TaxCode> source)
		{
			var taxCategory = source?.Filters?[0]?.Values?[0]?.Term ?? "";
			var taxCategorySearch = taxCategory.Trim('0');
			var search = source.Search;
			var filter = search != "" ? $"isActive eq true and taxCode startsWith '{taxCategorySearch}' and (taxCode contains '{search}' OR description contains '{search}')" : $"isActive eq true and taxCode startsWith '{taxCategorySearch}'";
			return new TaxCodeListArgs()
			{
				Filter = filter,
				Top = source.PageSize,
				Skip = (source.Page - 1) * source.PageSize,
				CodeCategory = taxCategory,
				OrderBy = null
			};
		}
	}
}
