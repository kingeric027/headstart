import { BuyerAddress, ListBuyerAddress } from 'marketplace';

export const getSuggestedAddresses = (ex, address: BuyerAddress) => {
    let suggestedAddresses: ListBuyerAddress;
    ex.error.Errors.forEach(err => {
        if (err.ErrorCode === "blocked by web hook") {
            err.Data.Body.SuggestedValidAddresses.forEach(suggestion => {
                suggestion.Shipping = true;
                suggestion.Billing = true;
                suggestion.FirstName = address.FirstName;
                suggestion.LastName = address.LastName;
                suggestion.Phone = address.Phone;
            });
        }
        suggestedAddresses = err.Data.Body.SuggestedValidAddresses;
    });
    return suggestedAddresses;
}