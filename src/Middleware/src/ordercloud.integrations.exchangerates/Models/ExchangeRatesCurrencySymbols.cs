using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ordercloud.integrations.exchangerates
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CurrencySymbols
    {
        CAD,
        HKD,
        ISK,
        PHP,
        DKK,
        HUF,
        CZK,
        GBP,
        RON,
        SEK,
        IDR,
        INR,
        BRL,
        RUB,
        HRK,
        JPY,
        THB,
        CHF,
        EUR,
        MYR,
        BGN,
        TRY,
        CNY,
        NOK,
        NZD,
        ZAR,
        USD,
        MXN,
        SGD,
        AUD,
        ILS,
        KRW,
        PLN
    }

    public class CurrencyDisplay
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
    }
    public static class SymbolLookup
    {
        public static IDictionary<CurrencySymbols, CurrencyDisplay> CurrencySymbolLookup = new Dictionary<CurrencySymbols, CurrencyDisplay>()
        {
            { CurrencySymbols.AUD, new CurrencyDisplay() { Name = "Australian Dollar", Symbol = "$"}},
            { CurrencySymbols.BGN, new CurrencyDisplay() { Name = "Bulgaria Lev", Symbol = "лв"}},
            { CurrencySymbols.RON, new CurrencyDisplay() { Name = "Romanian Leu", Symbol = "lei"}},
            { CurrencySymbols.BRL, new CurrencyDisplay() { Name = "Brazil Real", Symbol = "R$"}},
            { CurrencySymbols.CAD, new CurrencyDisplay() { Name = "Canada Dollar", Symbol = "$"}},
            { CurrencySymbols.HKD, new CurrencyDisplay() { Name = "Hong Kong Dollar", Symbol = "$"}},
            { CurrencySymbols.ISK, new CurrencyDisplay() { Name = "Iceland Krona", Symbol = "kr"}},
            { CurrencySymbols.PHP, new CurrencyDisplay() { Name = "Phillipines Peso", Symbol = "₱"}},
            { CurrencySymbols.DKK, new CurrencyDisplay() { Name = "Denmark Krone", Symbol = "kr"}},
            { CurrencySymbols.HUF, new CurrencyDisplay() { Name = "Hungary Forint", Symbol = "ft"}},
            { CurrencySymbols.CZK, new CurrencyDisplay() { Name = "Czech Republic Koruna", Symbol = "Kč"}},
            { CurrencySymbols.GBP, new CurrencyDisplay() { Name = "United Kingdom Pound", Symbol = "£"}},
            { CurrencySymbols.SEK, new CurrencyDisplay() { Name = "Sweden Krona", Symbol = "kr"}},
            { CurrencySymbols.IDR, new CurrencyDisplay() { Name = "Indonesia Rupiah", Symbol = "Rp"}},
            { CurrencySymbols.INR, new CurrencyDisplay() { Name = "India Rupee", Symbol = "₹"}},
            { CurrencySymbols.RUB, new CurrencyDisplay() { Name = "Russia Ruble", Symbol = "₽"}},
            { CurrencySymbols.HRK, new CurrencyDisplay() { Name = "Croatia Kuna", Symbol = "kn"}},
            { CurrencySymbols.JPY, new CurrencyDisplay() { Name = "Japan Yen", Symbol = "¥"}},
            { CurrencySymbols.THB, new CurrencyDisplay() { Name = "Thailand Baht", Symbol = "฿"}},
            { CurrencySymbols.CHF, new CurrencyDisplay() { Name = "Switzerland Franc", Symbol = "CHF"}},
            { CurrencySymbols.EUR, new CurrencyDisplay() { Name = "Euro Member Countries", Symbol = "€"}},
            { CurrencySymbols.MYR, new CurrencyDisplay() { Name = "Malaysia Ringgit", Symbol = "RM"}},
            { CurrencySymbols.TRY, new CurrencyDisplay() { Name = "Turkey Lira", Symbol = "₺"}},
            { CurrencySymbols.CNY, new CurrencyDisplay() { Name = "China Yuan Renminbi", Symbol = "/元"}},
            { CurrencySymbols.NOK, new CurrencyDisplay() { Name = "Norway Krone", Symbol = "kr"}},
            { CurrencySymbols.NZD, new CurrencyDisplay() { Name = "New Zealand Dollar", Symbol = "$"}},
            { CurrencySymbols.ZAR, new CurrencyDisplay() { Name = "South Africa Rand", Symbol = "R"}},
            { CurrencySymbols.USD, new CurrencyDisplay() { Name = "United States Dollar", Symbol = "$"}},
            { CurrencySymbols.MXN, new CurrencyDisplay() { Name = "Mexico Peso", Symbol = "$"}},
            { CurrencySymbols.SGD, new CurrencyDisplay() { Name = "Singapore Dollar", Symbol = "$"}},
            { CurrencySymbols.ILS, new CurrencyDisplay() { Name = "Israel Shekel", Symbol = "₪"}},
            { CurrencySymbols.KRW, new CurrencyDisplay() { Name = "Korea (South) Won", Symbol = "₩"}},
            { CurrencySymbols.PLN, new CurrencyDisplay() { Name = "Poland Zloty", Symbol = "zł"}},
        };
    }
}
