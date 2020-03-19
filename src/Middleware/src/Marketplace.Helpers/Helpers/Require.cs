﻿using System;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Models;

namespace Marketplace.Helpers
{
    public static class Require
    {
        /// <summary>
        /// Throws an error if condition is false. HTTP status is defined in the ErrorCode object. 
        /// Example usage: Require.That(check == true, ErrorCodes.Payment.AlreadyPaid, SomeObject); 
        /// See ErrorCodes.txt for error definitions 
        /// </summary>
        public static void That<TModel>(bool condition, ErrorCode<TModel> errorCode, TModel model)
        {
            if (!condition)
            {
                throw new ApiErrorException(errorCode, model);
            }
        }
        /// <summary>
        /// Throws an error if condition is false. Error model is build lazily. HTTP status is defined in the ErrorCode object. 
        /// Example usage: Require.That(check == true, ErrorCodes.Payment.AlreadyPaid, () => new ErrorModel(x,y,z)); 
        /// See ErrorCodes.txt for error definitions 
        /// </summary>
        public static void That<TModel>(bool condition, ErrorCode<TModel> errorCode, Func<TModel> buildModel)
        {
            if (!condition)
            {
                throw new ApiErrorException(errorCode, buildModel());
            }
        }
        /// <summary>
        /// Overload for when you don't need to pass back an object
        /// </summary>
        public static void That(bool condition, ErrorCode errorCode)
        {
            if (!condition)
            {
                throw new ApiErrorException(errorCode, null);
            }
        }

        /// <summary>
        /// Check any condition like Require.That, except throws a NotAuthorizedException (403 forbidden)
        /// </summary>
        public static void Allowed(bool condition, ErrorCode errorCode)
        {
            if (!condition)
                throw new ApiErrorException(errorCode, null);
        }

        /// <summary>
		/// New version of Require.Exists that takes in an ErrorCode
		/// </summary>
		public static void Exists<TModel>(object thing, ErrorCode<TModel> errorCode, TModel model)
        {
            if (thing == null)
                throw new ApiErrorException(errorCode, model);
        }
        /// <summary>
        /// For when you don't need to pass back an object
        /// </summary>
        public static void Exists(object thing, ErrorCode errorCode)
        {
            if (thing == null)
                throw new ApiErrorException(errorCode, null);
        }

        /// <summary>
        /// Throws a NotFoundException with default message if object is null (translates to 404 in the API)
        /// </summary>
        public static T MustExist<T>(this T thing, string interopID) where T : class
        {
            return MustExist(thing, typeof(T).Name, interopID);
        }

        /// <summary>
        /// Throws a NotFoundException with default message if object is null (translates to 404 in the API)
        /// </summary>
        public static T MustExist<T>(this T thing, string thingName, string interopID) where T : class
        {
            if (thing == null)
                throw new ApiErrorException.NotFoundException(thingName, interopID);
            return thing;
        }

        /// <summary>
        /// Throws a NotFoundException with default message if object is null (translates to 404 in the API)
        /// </summary>
        public static T MustExist<T>(this T? thing, string thingName, string interopID) where T : struct
        {
            if (thing == null)
                throw new ApiErrorException.NotFoundException(thingName, interopID);
            return thing.Value;
        }
    }
}