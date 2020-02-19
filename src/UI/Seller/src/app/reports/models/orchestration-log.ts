export interface OrchestrationLog {
  id: string;
  timeStamp: Date;
  ErrorType: OrchestrationErrorType;
  Message: string;
  ResourceId: string;
  RecordId: string;
  RecordType: RecordType;
  Action: Action;
  Current: any;
  Cache: any;
  Diff: any;
}

export enum OrchestrationErrorType {
  WorkItemDefinition,
  QueuedGetError,
  CachedGetError,
  DiffCalculationError,
  ActionEvaluationError,
  CacheUpdateError,
  QueueCleanupError,
  SyncCommandError,
  CreateExistsError,
  CreateGeneralError,
  UpdateGeneralError,
  PatchGeneralError,
  GetGeneralError,
}

export enum RecordType {
  Product,
  PriceSchedule,
  Spec,
  SpecOption,
  SpecProductAssignment,
  ProductFacet,
  Buyer,
  User,
  UserGroup,
  Address,
  CostCenter,
  UserGroupAssignment,
  AddressAssignment,
  CatalogAssignment,
  Catalog,
}

export enum Action {
  Ignore,
  Create,
  Update,
  Patch,
  Delete,
  Get,
}
