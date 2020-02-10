﻿using System.Collections.Generic;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Common.Services.Zoho.Parser;
using Marketplace.Common.Services.Zoho.Util;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.API
{
    /// <summary>
    /// Class BankaccountsApi is used to create a bank ZohoAccount or credit card for the organisation.
    /// Used to get <br></br> The list of accounts.<br></br>The details of the prticular bank account.
    /// It is used to change the status of an account as active to inactive and vice versa.
    /// It is used to update or delete the bank account.
    /// </summary>
    public class BankAccountsApi:Api
    {
        /// <summary>
        /// The base ZohoAddress
        /// </summary>
        static string baseAddress =baseurl + "/bankaccounts";


        /// <summary>
        /// Initializes a new instance of the <see cref="BankAccountsApi" /> class.
        /// </summary>
        /// <param name="auth_token">The user's auth_token is used for the authentication purpose.</param>
        /// <param name="organization_Id">The organization_ identifier is used to define the current working organisation.</param>
        public BankAccountsApi(string auth_token, string organization_Id): base(auth_token, organization_Id)
        {
           
        }
        /// <summary>
        /// Gets the bank accounts.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ZohoBankAccountList.</returns>
        public ZohoBankAccountList GetBankAccounts(Dictionary<object, object> parameters)
        {
            string url = baseAddress;
            var responce = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return BankAccountParser.getBankAccountList(responce);
        }

        /// <summary>
        /// Gets the details of the specified account.
        /// </summary>
        /// <param name="account_id">The specified account identifier .</param>
        /// <returns>Returns the Bankaccount object.</returns>
        public ZohoBankAccount Get(string account_id)
        {
            string url = baseAddress + "/" + account_id;
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return BankAccountParser.getBankAccount(responce);
            
        }

        /// <summary>
        /// Creates the Bank account or credit card acount with the specified information.
        /// The specified information must contains the accountName, accountType parameters as a mandatory.
        /// </summary>
        /// <param name="new_account_info">The new_account_info is the object with account_name,account_type as mandatory parameters.</param>
        /// <returns>Bankaccount object.</returns>
        public ZohoBankAccount Create(ZohoBankAccount new_account_info)
        {
            string url = baseAddress;
            var json = JsonConvert.SerializeObject(new_account_info);
            var requestBody = new Dictionary<object, object>();
            requestBody.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(requestBody));
            return BankAccountParser.getBankAccount(responce);
        }

        /// <summary>
        /// Updates the details of the specified account.
        /// </summary>
        /// <param name="account_id">The account_id is the identifier of the acoount on which the modifications has to be applied .</param>
        /// <param name="update_info">The update_info is the Bankaccount object which is having the modification details.</param>
        /// <returns>Bankaccount object.</returns>
        public ZohoBankAccount Update(string account_id, ZohoBankAccount update_info)
        {
            string url = baseAddress + "/" + account_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonString = new Dictionary<object, object>();
            jsonString.Add("JSONString", json);
            var responce = ZohoHttpClient.put(url, getQueryParameters(jsonString));
            return BankAccountParser.getBankAccount(responce);
        }
        /// <summary>
        /// Deletes the specified bankaccount from the organisation.
        /// </summary>
        /// <param name="account_id">The account_id is the identifier of the account which has to be deleted.</param>
        /// <returns>System.String.<br>The success message is "The account has been deleted."</br></returns>
        public string Delete(string account_id)
        {
            string url = baseAddress + "/" + account_id;
            var responce = ZohoHttpClient.delete(url, getQueryParameters());
            return BankAccountParser.getMessage(responce);
        }

        /// <summary>
        /// Deactivates the account.
        /// </summary>
        /// <param name="account_id">The account_id is the identifier of the bankaccount which has to be deactivated.</param>
        /// <returns>System.String.<br>The success message is "The account has been marked as inactive."</br></returns>
        public string DeactivateAccount(string account_id)
        {
            string url = baseAddress + "/" + account_id + "/inactive";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return BankAccountParser.getMessage(responce);
        }
        /// <summary>
        /// Activates the bankaccount.
        /// </summary>
        /// <param name="account_id">The account_id is the identifier of the bankaccount to which the status has to be changed.</param>
        /// <returns>System.String.<br>The success message is "The account has been marked as active."</br></returns>
        public string ActivateAccount(string account_id)
        {
            string url = baseAddress + "/" + account_id + "/active";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return BankAccountParser.getMessage(responce);
        }

        /// <summary>
        /// Gets the details of previously imported statement for the account.
        /// </summary>
        /// <param name="account_id">The account_id is the acount identifier for which requiring the previously imported statement.</param>
        /// <returns>ZohoStatement object.</returns>
        public ZohoStatement GetLastImportedStatement(string account_id)
        {
            string url = baseAddress + "/" + account_id + "/statement/lastimported";
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return BankAccountParser.getStatement(responce);
        }

        /// <summary>
        /// Deletes the statement that was previously imported.
        /// </summary>
        /// <param name="account_id">The account_id is the acount identifier for which requiring the previously imported statement.</param>
        /// <param name="statement_id">The statement_id is the identifier of the statement.</param>
        /// <returns>System.String.</returns>
        public string DeleteLastImportedStatement(string account_id, string statement_id)
        {
            string url = baseAddress + "/" + account_id + "/statement/"+statement_id;
            var responce = ZohoHttpClient.delete(url, getQueryParameters());
            return BankAccountParser.getMessage(responce);
        }
    }
}
