export class ErrorConstants {
  public static readonly orderSubmittedError = 'This order has already been submitted';
  public static readonly orderNotAccessibleError = 'You no longer have access to this order';
}

export function getPaymentError(errorReason: string): string {
  const reason = errorReason.replace('AVS', 'Address Verification') // AVS isn't likely something to be understood by a layperson
  return `The authorization for your payment was declined by the processor due to ${reason}. 
    Please reenter your information or use a different card.`
}
