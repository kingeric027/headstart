using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common;
using Marketplace.Common.Services;
using Marketplace.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using NUnit.Framework;
using NSubstitute;
using OrderCloud.SDK;
using Marketplace.Helpers.Services;

namespace Marketplace.Tests
{

    // NOTE: in order for these tests to pass container "images" must exist
    // this is due to a bug in BlobService that doesn't create container if it doesnt exist
    public class ContentManagementServiceTests
    {
        private const string CONTAINER_NAME = "images";

        [Test]
        public async Task ShouldUploadProductWithNoProductImages()
        {
            // Arrange
            var setup = GetTestSetup();
            var marketplaceID = "mockMarketplaceID";
            var productID = "mockProductID";
            var token = "mockToken";
            var expectedBlobID = $"{marketplaceID}/products/{productID}-1";
            await setup.blob.Delete(expectedBlobID); // TODO: this should be moved to a SetUp() method once BlobService can list https://github.com/nunit/docs/wiki/SetUp-and-TearDown

            setup.occlient.Products.GetAsync<Product<ProductXp>>("mockProductID", "mockToken")
                .Returns(Task.FromResult(new Product<ProductXp>()));
            var file = GetMockFile();

            // Act
            var sut = setup.contentManagementService;
            var result = await sut.UploadProductImage(file, marketplaceID, productID, token);

            // Assert
            Assert.AreEqual("patched-product", result.ID);
            var blobfile = await setup.blob.Get(expectedBlobID);
            Assert.AreEqual(blobfile, "This is a mock file");
        }

        /// <summary>
        /// Returns an object with mocked service and dependencies to reduce boilerplate
        /// Idea is that this object will have default return values suitable for most test cases
        /// If a test case needs to deviate from that, an overriden service can be provided
        /// </summary>
        private TestSetup GetTestSetup(AppSettings settingsSub = null, IOrderCloudClient ocSub = null, IBlobService blobSub = null)
        {
            var settings = settingsSub ?? new AppSettings();
            if (settingsSub == null)
            {
                settings.BlobSettings = new BlobSettings()
                {
                    ConnectionString = TestConstants.StorageConnectionString
                };
            }

            var ocClient = ocSub ?? Substitute.For<IOrderCloudClient>();
            if (ocSub == null)
            {
                ocClient.Products.GetAsync<Product<ProductXp>>("mockProductID", "mockToken")
                    .Returns(Task.FromResult(Mocks.Products.ProductWithOneImage()));

                ocClient.Products.PatchAsync("mockProductID", Arg.Any<PartialProduct>(), "mockToken")
                    .Returns(Task.FromResult(Mocks.Products.PatchResponse()));
            }

            var blob = blobSub ?? new BlobService(new BlobServiceConfig
            {
                ConnectionString = TestConstants.StorageConnectionString,
                Container = CONTAINER_NAME
            });

            return new TestSetup()
            {
                contentManagementService = new ContentManagementService(settings, ocClient),
                settings = settings,
                occlient =  ocClient,
                blob = blob
            };
        }

        private class TestSetup
        {
            // service under test
            public ContentManagementService contentManagementService { get; set; }
            
            // dependencies of service under test
            public AppSettings settings { get; set; }
            public IOrderCloudClient occlient { get; set; }
            public IBlobService blob { get; set; }
        }

        private IFormFile GetMockFile()
        {
            var fileMock = Substitute.For<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("This is a mock file");
            writer.Flush();
            ms.Position = 0;
            fileMock.OpenReadStream().ReturnsForAnyArgs(ms);
            fileMock.Name.ReturnsForAnyArgs("mock-image.png");
            fileMock.ContentType.ReturnsForAnyArgs("text/plain");
            fileMock.FileName.ReturnsForAnyArgs("mock-image.png");
            fileMock.Length.ReturnsForAnyArgs(ms.Length);
            return fileMock;
        }
    }
}
