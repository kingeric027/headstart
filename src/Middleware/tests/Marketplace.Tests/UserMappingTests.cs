using Marketplace.Common.Mappers;
using Marketplace.Common.Models;
using Marketplace.Common.Services.AnytimeDashboard.Models;
using Marketplace.Common.Services.WaxingDashboard.Models;
using Marketplace.Models;
using NUnit.Framework;
using ordercloud.integrations.exchangerates;
using System;
using System.Collections;

namespace Marketplace.Tests
{
    public class UserMappingTests
    {
        private void assert_users_are_equal(SyncUser expectedUser, SyncUser user)
		{
            Assert.AreEqual(expectedUser.ID, user.ID);
            Assert.AreEqual(expectedUser.Active, user.Active);
            Assert.AreEqual(expectedUser.IsAdmin, user.IsAdmin);
            Assert.AreEqual(expectedUser.ShouldSync, user.ShouldSync);
            Assert.AreEqual(expectedUser.Username, user.Username);
            Assert.AreEqual(expectedUser.Email, user.Email);
            Assert.AreEqual(expectedUser.FirstName, user.FirstName);
            Assert.AreEqual(expectedUser.LastName, user.LastName);
            Assert.AreEqual(expectedUser.xp.Country, user.xp.Country);
        }

        [Test]
		[TestCaseSource(typeof(UserFactory), nameof(UserFactory.AFUsers))]
		public void map_anytime_staff_to_user(string buyerID, string country, AFStaff staff, SyncUser expectedUser)
		{
            var location = new HSBuyerLocation() { Address = new HSAddressBuyer() { Country = country } };
            var user = UserMapper.MapToUser(buyerID, location, staff);
            assert_users_are_equal(expectedUser, user);
        }

        [Test]
        [TestCaseSource(typeof(UserFactory), nameof(UserFactory.WTCUsers))]
        public void map_waxing_staff_to_user(string buyerID, WTCStaff staff, SyncUser expectedUser)
        {
            var user = UserMapper.MapToUser(buyerID, staff);
            assert_users_are_equal(expectedUser, user);
        }

        public class UserFactory
		{
            public static IEnumerable WTCUsers
            {
                get
                {
                    yield return new TestCaseData("0005",
                        new WTCStaff()
                        {
                            id = 17575586,
                            firstName = "Lindsay",
                            lastName = "Cress",
                            email = "fakename@rocketMail.com",
                            userType = "Cerologist",
                            status = "Active",
                        },
                        new SyncUser()
                        {
                            ShouldSync = false,
                            IsAdmin = true,
                            ID = "0005-W17575586",
                            Username = "W-fakename@rocketMail.com",
                            FirstName = "Lindsay",
                            LastName = "Cress",
                            Email = "fakename@rocketMail.com",
                            Active = true,
                            xp = new UserXp()
                            {
                                Country = "US"
                            }
                        }
                    );
                }
            }

            public static IEnumerable AFUsers
            {
                get
                {
                    yield return new TestCaseData("0006", "US",
                        new AFStaff()
                        {
                            id = "1084804",
                            firstName = "Tayler",
                            lastName = "Johnson",
                            email = "somename@anytimefitness.com",
                            type = "Regional Manager",
                            language = null,
                            username = "somename@anytimefitness.com",
                            isDeleted = false,
                            updated = DateTime.Now
                        },
                        new SyncUser()
                        {
                            ShouldSync = true,
                            IsAdmin = true,
                            ID = "0006-A1084804",
                            Username = "A-somename@anytimefitness.com",
                            FirstName = "Tayler",
                            LastName= "Johnson",
                            Email = "somename@anytimefitness.com",
                            Active = true,
                            xp = new UserXp()
							{
                                Country = "US"
							}
                        }
                    );
                }
            }
        }
    }
}
