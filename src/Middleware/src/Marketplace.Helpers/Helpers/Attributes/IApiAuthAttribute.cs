using OrderCloud.SDK;

namespace Marketplace.Helpers.Attributes
{
    public interface IApiAuthAttribute
    {
        ApiRole[] ApiRoles { get; }
    }

    public interface IErrorCode
    {
        string Code { get; set; }
        int HttpStatus { get; set; }
        string DefaultMessage { get; set; }
    }
}
