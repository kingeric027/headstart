using Headstart.Common.Mappers;
using Headstart.Common.Services.AnytimeDashboard.Models;
using Headstart.Common.Services.WaxingDashboard.Models;
using Headstart.Models;
using NUnit.Framework;
using ordercloud.integrations.exchangerates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static Headstart.Common.Services.AnytimeDashboard.Models.AFClub;

namespace Headstart.Tests
{
    public class LocationMappingTests
    {
        private void assert_locations_are_equal(SyncLocation expectedlocation, SyncLocation location)
        {
            Assert.AreEqual(expectedlocation.FranchiseeID, location.FranchiseeID);
            Assert.AreEqual(expectedlocation.ShouldSync, location.ShouldSync);
            Assert.AreEqual(expectedlocation.Address.ID, location.Address.ID);
            Assert.AreEqual(expectedlocation.Address.Phone, location.Address.Phone);
            Assert.AreEqual(expectedlocation.Address.City, location.Address.City);
            Assert.AreEqual(expectedlocation.Address.State, location.Address.State);
            Assert.AreEqual(expectedlocation.Address.Zip, location.Address.Zip);
            Assert.AreEqual(expectedlocation.Address.Street1, location.Address.Street1);
            Assert.AreEqual(expectedlocation.Address.Street2, location.Address.Street2);
            Assert.AreEqual(expectedlocation.Address.Country, location.Address.Country);
            Assert.AreEqual(expectedlocation.Address.AddressName, location.Address.AddressName);
            Assert.AreEqual(expectedlocation.Address.CompanyName, location.Address.CompanyName);
            Assert.AreEqual(expectedlocation.Address.xp.LocationID, location.Address.xp.LocationID);
            Assert.AreEqual(expectedlocation.Address.xp.Email, location.Address.xp.Email);
            Assert.AreEqual(expectedlocation.Address.xp.BillingNumber, location.Address.xp.BillingNumber);
            Assert.AreEqual(expectedlocation.Address.xp.Status, location.Address.xp.Status);
            Assert.AreEqual(expectedlocation.Address.xp.OpeningDate, location.Address.xp.OpeningDate);
            Assert.AreEqual(expectedlocation.Address.xp.LegalEntity, location.Address.xp.LegalEntity);
            Assert.AreEqual(expectedlocation.Address.xp.PrimaryContactName, location.Address.xp.PrimaryContactName);
            Assert.AreEqual(expectedlocation.UserGroup.ID, location.UserGroup.ID);
            Assert.AreEqual(expectedlocation.UserGroup.Name, location.UserGroup.Name);
            Assert.AreEqual(expectedlocation.UserGroup.Description, location.UserGroup.Description);
            Assert.AreEqual(expectedlocation.UserGroup.xp.Country, expectedlocation.UserGroup.xp.Country);
            Assert.AreEqual(expectedlocation.UserGroup.xp.Currency, expectedlocation.UserGroup.xp.Currency);
            Assert.AreEqual(expectedlocation.UserGroup.xp.Type, expectedlocation.UserGroup.xp.Type);
        }

        [Test]
        [TestCaseSource(typeof(LocationFactory), nameof(LocationFactory.WTCLocations))]
        public void map_anytime_club_to_location(string buyerID, WTCStudio studio, SyncLocation expectedlocation)
        {
            var location = LocationMapper.MapToLocation(buyerID, studio);
            assert_locations_are_equal(expectedlocation, location);
        }

        [Test]
        [TestCaseSource(typeof(LocationFactory), nameof(LocationFactory.AFLocations))]
        public void map_waxing_studio_to_location(string buyerID, AFClub club, SyncLocation expectedlocation)
        {
            var location = LocationMapper.MapToLocation(buyerID, club);
            assert_locations_are_equal(expectedlocation, location);
        }

        public class LocationFactory
        {
            public static IEnumerable WTCLocations
            {
                get
                {
                    yield return new TestCaseData("0005",
                        new WTCStudio()
                        {
                            locationNumber = "W15",
                            locationName = "Indianapolis, IN",
                            address1 = "3855 E 96th St",
                            address2 = null,
                            city = "Indianapolis",
                            state = "IN",
                            country = "USA",
                            postCode = "46240",
                            email = "IndianapolisIN@waxingthecity.com",
                            phoneNumber = "(317) 759-2700",
                            openingDate = new DateTime(2014, 12, 08),
                            primaryContactName = "Tony Black",
                            openStatus = "Open",
                            status = "Active",
                            legalEntity = "Black Bare, LLC",
                        },
                        new SyncLocation()
                        {
                            FranchiseeID = "W15",                
                            ShouldSync = true,
                            Address = new HSAddressBuyer()
                            {
                                ID = "0005-W15",
                                Phone = "(317) 759-2700",
                                City = "Indianapolis",
                                State = "IN",
                                Zip = "46240",
                                Street1 = "3855 E 96th St",
                                Street2 = null,
                                Country = "US",
                                AddressName = "Indianapolis, IN",
                                CompanyName = "Waxing The City",
                                xp = new BuyerAddressXP()
                                {
                                    LocationID = "0005-W15",
                                    Email = "IndianapolisIN@waxingthecity.com",
                                    BillingNumber = null,
                                    Status = "Active",
                                    OpeningDate = new DateTime(2014, 12, 08),
                                    LegalEntity = "Black Bare, LLC",
                                    PrimaryContactName = "Tony Black"
                                }
                            },
                            UserGroup = new HSLocationUserGroup()
                            {
                                ID = "0005-W15",
                                Name = "Indianapolis, IN",
                                xp = new HSLocationUserGroupXp()
                                {
                                    Country = "US",
                                    Type = "BuyerLocation",
                                    Currency = CurrencySymbol.USD
                                }
                            }
                        }
                    );
                }
            }

            public static IEnumerable AFLocations
            {
                get
                {
                    yield return new TestCaseData("0006",
                        new AFClub()
                        {
                            id = "1084804",
                            afNumber = "135",
                            billingNumber = "1268",
                            name = "Grand Island",
                            phoneNumber = "(111) 111 - 4700",
                            email = "something@anytimefitness.com",
                            legalEntity = "Robinson Fitness, L.L.C.",
                            primaryContactName = "Ryan Robinson",
                            isDeleted = false,
                            openingDate = new DateTime(2014, 12, 08),
                            address = new AFAddress()
							{
                                city = "Grand Island",
                                stateProvince = "Nebraska",
                                postCode = "68803",
                                address = "3721 W 13th St",
                                address2 = "Ste B",
                                country = "US"
                            },
                            coordinates = new Coordinates()
							{
                                Latitude = 43.93121,
                                Longitude = -94.38884,
							},
                            status = new AFLocationStatus()
							{
                                id = "3",
                                description = "Open"
							}
                        },
                        new SyncLocation()
                        {
                            FranchiseeID = "1084804",
                            ShouldSync = true,
                            Address = new HSAddressBuyer()
                            {
                                ID = "0006-A1084804",
                                Phone = "(111) 111 - 4700",
                                City = "Grand Island",
                                State = "NE",
                                Zip = "68803",
                                Street1 = "3721 W 13th St",
                                Street2 = "Ste B",
                                Country = "US",
                                AddressName = "Grand Island",
                                CompanyName = "Anytime Fitness",
                                xp = new BuyerAddressXP()
                                {
                                    LocationID = "0006-A1084804",
                                    Email = "something@anytimefitness.com",
                                    BillingNumber = "1268",
                                    Status = "Open",
                                    OpeningDate = new DateTime(2014, 12, 08),
                                    LegalEntity = "Robinson Fitness, L.L.C.",
                                    PrimaryContactName = "Ryan Robinson"
                                }
                            },
                            UserGroup = new HSLocationUserGroup()
                            {
                                ID = "0006-A1084804",
                                Name = "Grand Island",
                                xp = new HSLocationUserGroupXp()
                                {
                                    Country = "US",
                                    Type = "BuyerLocation",
                                    Currency = CurrencySymbol.USD
                                }
                            }
                        }
                    );
                }
            }
        }
    }
}
