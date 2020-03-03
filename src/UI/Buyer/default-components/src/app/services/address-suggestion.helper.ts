import { BuyerAddress, ListBuyerAddress } from 'marketplace';

export const getSuggestedAddresses = (ex, address: BuyerAddress): ListBuyerAddress => {
  let isErrorFromOC = true;
  let suggestedAddresses: ListBuyerAddress;
  ex.error.Errors.forEach(err => {
    if (err.ErrorCode === 'blocked by web hook') {
      isErrorFromOC = false;
      err.Data.Body.SuggestedAddresses.forEach((suggestion: BuyerAddress) => {
        suggestion.Shipping = true;
        suggestion.Billing = true;
        suggestion.FirstName = address.FirstName;
        suggestion.LastName = address.LastName;
        suggestion.Phone = address.Phone;
        suggestion.Country = address.Country;
      });
    }
    suggestedAddresses = err.Data?.Body?.SuggestedAddresses;
  });
  if (isErrorFromOC) throw ex;
  return suggestedAddresses;
};
