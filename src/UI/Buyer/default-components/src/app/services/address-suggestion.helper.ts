import { ListBuyerAddress } from 'marketplace';

export const getSuggestedAddresses = (ex): ListBuyerAddress => {
  for (const err of ex.error.Errors) {
    if (err.ErrorCode === 'blocked by web hook') {
      return err.Data?.Body?.SuggestedAddresses;
    }
  }
  throw ex;
};
