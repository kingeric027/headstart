import { ListBuyerAddress } from '@ordercloud/angular-sdk';

export const getSuggestedAddresses = (ex): ListBuyerAddress => {
  for (const err of ex.error) {
    if (err.ErrorCode === 'blocked by web hook') {
        console.log(err.Data?.Body?.SuggestedAddresses)
      return err.Data?.Body?.SuggestedAddresses;
    }
  }
  throw ex;
};