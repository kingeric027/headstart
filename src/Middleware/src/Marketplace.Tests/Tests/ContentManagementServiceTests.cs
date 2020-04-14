﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common;
using Marketplace.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using NUnit.Framework;
using NSubstitute;
using OrderCloud.SDK;
using Marketplace.Helpers.Services;
using Marketplace.Models;
using Marketplace.Common.Queries;
using Marketplace.Common.Models;

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
            var file = GetMockFile();

            // Act
            var sut = setup.contentManagementService;
            var result = await sut.UploadProductImage(file, marketplaceID, productID, token);

            // Assert
            Assert.AreEqual("patched-product", result.Product.ID);
            var blobfile = await setup.blob.Get(expectedBlobID);
            Assert.AreEqual(blobfile, "This is a mock file");
        }

        /// <summary>
        /// Returns an object with mocked service and dependencies to reduce boilerplate
        /// Idea is that this object will have default return values suitable for most test cases
        /// If a test case needs to deviate from that, an overriden service can be provided
        /// </summary>
        private TestSetup GetTestSetup(AppSettings settingsSub = null, IOrderCloudClient ocSub = null, IBlobService blobSub = null, IImageQuery imageQuerySub = null, IImageProductAssignmentQuery imageProductAssignmentQuerySub = null)
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
                ocClient.Products.GetAsync<MarketplaceProduct>("mockProductID", "mockToken")
                    .Returns(Task.FromResult(Mocks.Products.ProductWithOneImage()));
                
                var mockSpecListPage = new ListPage<Spec>() { Meta = new ListPageMeta(), Items = new List<Spec>() };
                ocClient.Products.ListSpecsAsync("mockProductID", null, null, null, 1, 100, null, "mockToken")
                    .Returns(Task.FromResult(mockSpecListPage));
                
                var mockVariantListPage = new ListPage<MarketplaceVariant>() { Meta = new ListPageMeta(), Items = new List<MarketplaceVariant>() };
                ocClient.Products.ListVariantsAsync<MarketplaceVariant>("mockProductID", null, null, null, 1, 100, null, "mockToken")
                    .Returns(mockVariantListPage);

                ocClient.Products.PatchAsync<MarketplaceProduct>("mockProductID", Arg.Any<PartialProduct>(), "mockToken")
                    .Returns(Task.FromResult(Mocks.Products.PatchResponse()));
            }

            var blob = blobSub ?? new BlobService(new BlobServiceConfig
            {
                ConnectionString = TestConstants.StorageConnectionString,
                Container = CONTAINER_NAME
            });
            
            var imageQuery = imageQuerySub ?? Substitute.For<IImageQuery>();
            if (imageQuerySub == null)
            {
                imageQuery.Save(Arg.Any<Image>())
                    .Returns(Task.FromResult(new Image() { id = "blah" }));

                var mockImageListPage = new ListPage<Image>() { Meta = new ListPageMeta(), Items = new List<Image>() };
                imageQuery.GetProductImages("mockProductID").Returns(mockImageListPage);
            }

            var imageProductAssignmentQuery = imageProductAssignmentQuerySub ?? Substitute.For<IImageProductAssignmentQuery>();

            return new TestSetup()
            {
                contentManagementService = new ContentManagementService(settings, ocClient, imageQuery, imageProductAssignmentQuery),
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
