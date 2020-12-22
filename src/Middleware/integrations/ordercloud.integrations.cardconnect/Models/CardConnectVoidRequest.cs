namespace ordercloud.integrations.cardconnect
{
    public class CardConnectVoidRequest
    {
        public string merchid { get; set; }
        public string retref { get; set; }
        //  if equal to $0, the full amount is voided. Defaults to 0 to support full amount voiding
        public decimal? amount { get; set; } = 0M;
    }

    public class CardConnectVoidResponse : CardConnectResponse
    {
        public string merchid { get; set; }
        public decimal? amount { get; set; }
        public string orderId { get; set; }
        public string retref { get; set; }
        public string authcode { get; set; }
    }

}
