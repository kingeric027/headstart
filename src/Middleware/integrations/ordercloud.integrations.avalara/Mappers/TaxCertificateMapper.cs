using Avalara.AvaTax.RestClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.avalara
{
	public static class TaxCertificateMapper
	{
		// See https://sandbox-rest.avatax.com/api/v2/definitions/certificateexemptreasons for all available reasons
		private static readonly string RESALE_EXEMPTION_REASON = "RESALE";

		// Resale Exemption Certificates
		public static CertificateModel Map(TaxCertificate source)
		{
			return new CertificateModel()
			{
				id = source.ID,
				signedDate = source.SignedDate.UtcDateTime,
				expirationDate = source.ExpirationDate.UtcDateTime,
				pdf = source.Base64UrlEncodedPDF,
				exemptPercentage = 100,
				exemptionNumber = source.ExemptionNumber,
				filename = source.FileName,
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

		public static TaxCertificate Map(CertificateModel source, string base64pdf)
		{
			return new TaxCertificate()
			{
				ID = source.id ?? 0,
				SignedDate = source.signedDate,
				ExpirationDate = source.expirationDate,
				ExemptionNumber = source.exemptionNumber,
				ExposureZoneName = source.exposureZone.name,
				FileName = source.filename,
				Base64UrlEncodedPDF = base64pdf
			};
		}
	}
}
