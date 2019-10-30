using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Models;

namespace Marketplace.Common.Extensions
{
    public static class OrchestrationExtensions
    {
        public static string BuildPath(this IOrchestrationObject obj, string supplierId)
        {
            return $"{supplierId}/{obj.Type()}/{obj.ID}".ToLower();
        }

        public static RecordType Type(this IOrchestrationObject obj)
        {
            switch (obj.GetType().Name)
            {
                case nameof(OrchestrationProduct):
                    return RecordType.Product;
                case nameof(OrchestrationProductFacet):
                    return RecordType.ProductFacet;
                case nameof(OrchestrationPriceSchedule):
                    return RecordType.PriceSchedule;
                case nameof(OrchestrationSpec):
                    return RecordType.Spec;
                case nameof(OrchestrationSpecOption):
                    return RecordType.SpecOption;
                case nameof(OrchestrationSpecProductAssignment):
                    return RecordType.SpecProductAssignment;
                default:
                    throw new ApiErrorException(ErrorCodes.All["UnrecognizedType"], obj);
            }
        }
    }
}
