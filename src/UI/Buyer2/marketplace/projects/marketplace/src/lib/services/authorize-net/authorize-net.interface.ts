export interface AuthorizeCardSuccess {
  ResponseBody: AuthorizeCardResponseBody;
}

interface AuthorizeCardResponseBody {
  ChargeStatus: string;
  CreditCardID: string;
  PaymentID: string;
  TransactionID: string;
  Messages: AuthorizeResponseMessage[];
}

interface AuthorizeResponseMessage {
  code: number;
  description: string;
}
