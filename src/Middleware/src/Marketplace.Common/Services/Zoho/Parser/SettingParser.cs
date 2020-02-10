using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of SettingsApi.
    /// </summary>
    class SettingsParser
    {

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoPreferences getPreferences(HttpResponseMessage response)
        {
            var preferences = new ZohoPreferences();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("preferences"))
            {
                preferences = JsonConvert.DeserializeObject<ZohoPreferences>(jsonObj["preferences"].ToString());
            }
            return preferences;
        }

        internal static ZohoNotesAndTerms getNotesAndTerms(HttpResponseMessage response)
        {
            var notesAndTerms = new ZohoNotesAndTerms();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("notes_and_terms"))
            {
                notesAndTerms = JsonConvert.DeserializeObject<ZohoNotesAndTerms>(jsonObj["notes_and_terms"].ToString());
            }
            return notesAndTerms;
        }

        internal static ZohoTaxList getTaxList(HttpResponseMessage response)
        {
            var taxList = new ZohoTaxList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("taxes"))
            {
                var taxArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["taxes"].ToString());
                foreach(var taxObj in taxArray)
                {
                    var tax = new ZohoTax();
                    tax = JsonConvert.DeserializeObject<ZohoTax>(taxObj.ToString());
                    taxList.Add(tax);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                taxList.ZohoPageContext = pageContext;
            }
            return taxList;
        }

        internal static ZohoTax getTax(HttpResponseMessage response)
        {
            var tax = new ZohoTax();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("tax"))
            {
                tax = JsonConvert.DeserializeObject<ZohoTax>(jsonObj["tax"].ToString());
            }
            return tax;
        }

        internal static ZohoTaxGroup getTaxGroup(HttpResponseMessage response)
        {
            var taxgroup = new ZohoTaxGroup();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("tax_group"))
            {
                taxgroup = JsonConvert.DeserializeObject<ZohoTaxGroup>(jsonObj["tax_group"].ToString());
            }
            return taxgroup;
        }

        internal static ZohoOpeningBalance getOpeningBalance(HttpResponseMessage response)
        {
            var openingBalance = new ZohoOpeningBalance();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("opening_balance"))
            {
                openingBalance = JsonConvert.DeserializeObject<ZohoOpeningBalance>(jsonObj["opening_balance"].ToString());
            }
            return openingBalance;
        }

        internal static ZohoAutoReminderList getAutoReminderList(HttpResponseMessage response)
        {
            var reminderList = new ZohoAutoReminderList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("autoreminders"))
            {
                var reminderArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["autoreminders"].ToString());
                foreach(var reminderObj in reminderArray)
                {
                    var reminder = new ZohoAutoReminder();
                    reminder = JsonConvert.DeserializeObject<ZohoAutoReminder>(reminderObj.ToString());
                    reminderList.Add(reminder);
                }
            }
            return reminderList;
        }

        internal static ZohoAutoReminderAndPlaceHolders getAutoReminderAndPlaceHolders(HttpResponseMessage response)
        {
            var reminderAndPlaceHolders = new ZohoAutoReminderAndPlaceHolders();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("autoreminder"))
            {
                var reminder = new ZohoAutoReminder();
                reminder = JsonConvert.DeserializeObject<ZohoAutoReminder>(jsonObj["autoreminder"].ToString());
                reminderAndPlaceHolders.autoreminder = reminder;
            }
            if (jsonObj.ContainsKey("placeholders"))
            {
                var placeHolders = new ZohoPlaceHolders();
                placeHolders = JsonConvert.DeserializeObject<ZohoPlaceHolders>(jsonObj["placeholders"].ToString());
                reminderAndPlaceHolders.placeholders = placeHolders;
            }
            return reminderAndPlaceHolders;
        }

        internal static ZohoManualReminderList getManualReminderList(HttpResponseMessage response)
        {
            var reminderList = new ZohoManualReminderList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("manualreminders"))
            {
                var reminderArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["manualreminders"].ToString());
                foreach (var reminderObj in reminderArray)
                {
                    var reminder = new ZohoManualReminder();
                    reminder = JsonConvert.DeserializeObject<ZohoManualReminder>(reminderObj.ToString());
                    reminderList.Add(reminder);
                }
            }
            return reminderList;
        }

        internal static ZohoManualReminderAndPlaceHolders getManualReminderAndPlaceHolders(HttpResponseMessage response)
        {
            var reminderAndPlaceHolders = new ZohoManualReminderAndPlaceHolders();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("manualreminder"))
            {
                var reminder = new ZohoManualReminder();
                reminder = JsonConvert.DeserializeObject<ZohoManualReminder>(jsonObj["manualreminder"].ToString());
                reminderAndPlaceHolders.manualreminder = reminder;
            }
            if (jsonObj.ContainsKey("placeholders"))
            {
                var placeHolders = new ZohoPlaceHolders();
                placeHolders = JsonConvert.DeserializeObject<ZohoPlaceHolders>(jsonObj["placeholders"].ToString());
                reminderAndPlaceHolders.placeholders = placeHolders;
            }
            if (jsonObj.ContainsKey("show_org_address_as_one_field"))
            {
                reminderAndPlaceHolders.show_org_address_as_one_field = (bool)jsonObj["show_org_address_as_one_field"];
            }
            return reminderAndPlaceHolders;
        }

        internal static ZohoAutoReminder getAutoReminder(HttpResponseMessage response)
        {
            var autoReminder = new ZohoAutoReminder();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string,object>>(response.Content.ReadAsStringAsync().Result);
            if(jsonObj.ContainsKey("autoreminder"))
            {
                autoReminder = JsonConvert.DeserializeObject<ZohoAutoReminder>(jsonObj["autoreminder"].ToString());
            }
            return autoReminder;
        }

        internal static ZohoManualReminder getManualReminder(HttpResponseMessage response)
        {
            var manualReminder = new ZohoManualReminder();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("manualreminder"))
            {
                manualReminder = JsonConvert.DeserializeObject<ZohoManualReminder>(jsonObj["manualreminder"].ToString());
            }
            return manualReminder;
        }

        internal static ZohoTaxAuthorityList getTaxAuthorityList(HttpResponseMessage response)
        {
            var taxAuthorityList = new ZohoTaxAuthorityList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("tax_authorities"))
            {
                var taxAuthorityArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["tax_authorities"].ToString());
                foreach(var taxAuthorityObj in taxAuthorityArray)
                {
                    var taxAuthority = new ZohoTaxAuthority();
                    taxAuthority = JsonConvert.DeserializeObject<ZohoTaxAuthority>(taxAuthorityObj.ToString());
                    taxAuthorityList.Add(taxAuthority);
                }
            }
            return taxAuthorityList;
        }

        internal static ZohoTaxAuthority getTaxAuthority(HttpResponseMessage response)
        {
            var taxAuthority = new ZohoTaxAuthority();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("tax_authority"))
            {
                taxAuthority = JsonConvert.DeserializeObject<ZohoTaxAuthority>(jsonObj["tax_authority"].ToString());
            }
            return taxAuthority;
        }

        internal static ZohoTaxExemptionList getTaxExemptionList(HttpResponseMessage response)
        {
            var taxExemptionList = new ZohoTaxExemptionList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("tax_exemptions"))
            {
                var taxExemptionArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["tax_exemptions"].ToString());
                foreach(var taxExemptionObj in taxExemptionArray)
                {
                    var taxExemption = new ZohoTaxExemption();
                    taxExemption = JsonConvert.DeserializeObject<ZohoTaxExemption>(taxExemptionObj.ToString());
                    taxExemptionList.Add(taxExemption);
                }
            }
            return taxExemptionList;
        }

        internal static ZohoTaxExemption getTaxExemption(HttpResponseMessage response)
        {
            var taxExemption = new ZohoTaxExemption();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("tax_exemption"))
            {
                taxExemption = JsonConvert.DeserializeObject<ZohoTaxExemption>(jsonObj["tax_exemption"].ToString());
            }
            return taxExemption;
        }
    }
}
