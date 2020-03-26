using Marketplace.Helpers.Attributes;
using OrderCloud.SDK;

namespace Marketplace.Models.Misc
{
    [SwaggerModel]
	public class MessageNotification
	{
		public string BuyerID { get; set; }
		public string UserToken { get; set; }
		public User Recipient { get; set; }
		public MessageType MessageType { get; set; }
		public string[] CCRecipient { get; set; }
		public MessageConfigData ConfigData { get; set; }
		public MessageEventBody EventBody { get; set; }
	}

	public enum MessageType
    {
        OrderDeclined,
        OrderSubmitted,
        ShipmentCreated,
        ForgottenPassword,
        OrderSubmittedForYourApproval,
        OrderSubmittedForApproval,
        OrderApproved,
        OrderSubmittedForYourApprovalHasBeenApproved,
        OrderSubmittedForYourApprovalHasBeenDeclined,
        NewUserInvitation
    }

    public class MessageEventBody
    {
        public string Username { get; set; }
        public string PasswordRenewalAccessToken { get; set; }
        public string PasswordRenewalVerificationCode { get; set; }
        public string PasswordRenewalUrl { get; set; }
    }
    public class MessageConfigData
    {
        public MessageTypeConfig[] MessageTypeConfig { get; set; }
        public string ApiKey;
    }
    public class MessageTypeConfig
    {
        public string FromEmail { get; set; }
        public string MainContent { get; set; }
        public string MessageType { get; set; }
        public string Subject { get; set; }
        public string TemplateName { get; set; }
        public string Name { get; set; }
    }
}