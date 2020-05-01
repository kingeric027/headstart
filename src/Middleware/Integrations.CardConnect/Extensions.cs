using ordercloud.integrations.cardconnect.Models;
using OrderCloud.SDK;

namespace ordercloud.integrations.cardconnect.Mappers
{
    public static class Extensions
    {
        public static string ToCreditCardDisplay(this string value)
        {
            var result = $"{value.Substring(value.Length - 4, 4)}";
            return result;
        }

        public static bool IsValidCvv(this OrderCloudIntegrationsCreditCardPayment payment, BuyerCreditCard cc)
        {
            // if credit card is direct without using a saved card then consider it a ME card and should enforce CVV
            // saved credit cards for ME just require CVV
            return (payment.CreditCardDetails == null || payment.CVV != null) && (!cc.Editable || payment.CVV != null);
        }
    }
}
