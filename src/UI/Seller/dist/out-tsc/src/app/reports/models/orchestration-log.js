export var OrchestrationErrorType;
(function (OrchestrationErrorType) {
    OrchestrationErrorType[OrchestrationErrorType["WorkItemDefinition"] = 0] = "WorkItemDefinition";
    OrchestrationErrorType[OrchestrationErrorType["QueuedGetError"] = 1] = "QueuedGetError";
    OrchestrationErrorType[OrchestrationErrorType["CachedGetError"] = 2] = "CachedGetError";
    OrchestrationErrorType[OrchestrationErrorType["DiffCalculationError"] = 3] = "DiffCalculationError";
    OrchestrationErrorType[OrchestrationErrorType["ActionEvaluationError"] = 4] = "ActionEvaluationError";
    OrchestrationErrorType[OrchestrationErrorType["CacheUpdateError"] = 5] = "CacheUpdateError";
    OrchestrationErrorType[OrchestrationErrorType["QueueCleanupError"] = 6] = "QueueCleanupError";
    OrchestrationErrorType[OrchestrationErrorType["SyncCommandError"] = 7] = "SyncCommandError";
    OrchestrationErrorType[OrchestrationErrorType["CreateExistsError"] = 8] = "CreateExistsError";
    OrchestrationErrorType[OrchestrationErrorType["CreateGeneralError"] = 9] = "CreateGeneralError";
    OrchestrationErrorType[OrchestrationErrorType["UpdateGeneralError"] = 10] = "UpdateGeneralError";
    OrchestrationErrorType[OrchestrationErrorType["PatchGeneralError"] = 11] = "PatchGeneralError";
    OrchestrationErrorType[OrchestrationErrorType["GetGeneralError"] = 12] = "GetGeneralError";
})(OrchestrationErrorType || (OrchestrationErrorType = {}));
export var RecordType;
(function (RecordType) {
    RecordType[RecordType["Product"] = 0] = "Product";
    RecordType[RecordType["PriceSchedule"] = 1] = "PriceSchedule";
    RecordType[RecordType["Spec"] = 2] = "Spec";
    RecordType[RecordType["SpecOption"] = 3] = "SpecOption";
    RecordType[RecordType["SpecProductAssignment"] = 4] = "SpecProductAssignment";
    RecordType[RecordType["ProductFacet"] = 5] = "ProductFacet";
    RecordType[RecordType["Buyer"] = 6] = "Buyer";
    RecordType[RecordType["User"] = 7] = "User";
    RecordType[RecordType["UserGroup"] = 8] = "UserGroup";
    RecordType[RecordType["Address"] = 9] = "Address";
    RecordType[RecordType["CostCenter"] = 10] = "CostCenter";
    RecordType[RecordType["UserGroupAssignment"] = 11] = "UserGroupAssignment";
    RecordType[RecordType["AddressAssignment"] = 12] = "AddressAssignment";
    RecordType[RecordType["CatalogAssignment"] = 13] = "CatalogAssignment";
    RecordType[RecordType["Catalog"] = 14] = "Catalog";
})(RecordType || (RecordType = {}));
export var Action;
(function (Action) {
    Action[Action["Ignore"] = 0] = "Ignore";
    Action[Action["Create"] = 1] = "Create";
    Action[Action["Update"] = 2] = "Update";
    Action[Action["Patch"] = 3] = "Patch";
    Action[Action["Delete"] = 4] = "Delete";
    Action[Action["Get"] = 5] = "Get";
})(Action || (Action = {}));
//# sourceMappingURL=orchestration-log.js.map