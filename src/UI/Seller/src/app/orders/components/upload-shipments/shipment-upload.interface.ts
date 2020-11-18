import { Shipment } from '@ordercloud/angular-sdk';

export interface BatchProcessResult {
  ProcessFailureList: BatchProcessFailure[];
  InvalidRowFailureList: DocumentRowError[];
  SuccessfulList: Shipment[];
  Meta: BatchProcessSummary;
}

export interface BatchProcessFailure {
  Shipment: Shipment;
  Error: string;
}

export interface DocumentRowError {
  Row: number;
  ErrorMessage: String;
  Column: number;
}

export interface BatchProcessSummary {
  TotalCount: number;
  SuccessfulCount: number;
  ProcessFailureListCount: number;
  DocumentFailureListCount: number;
}
