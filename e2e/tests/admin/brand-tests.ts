import { t } from 'testcafe'
import testConfig from '../../testConfig'
import { adminTestSetup, adminClientSetup } from '../../helpers/test-setup'
import adminHeaderPage from '../../pages/admin/admin-header-page'
import mainResourcePage from '../../pages/admin/main-resource-page'
import brandDetailsPage from '../../pages/admin/brand-details-page'
import {
	deleteBuyerWithName,
	createDefaultBuyer,
	deleteBuyer,
} from '../../api-utils.ts/buyer-util'
import minorResourcePage from '../../pages/admin/minor-resource-page'
import catalogDetailsPage from '../../pages/admin/catalog-details-page'
import {
	deleteCatalogWithName,
	createDefaultCatalog,
	deleteCatalog,
} from '../../api-utils.ts/catalog-util'
import locationDetailsPage from '../../pages/admin/location-details-page'
import {
	deleteBuyerLocationWithName,
	createDefaultBuyerLocation,
	deleteBuyerLocation,
} from '../../api-utils.ts/buyer-locations-util'
import userDetailsPage from '../../pages/admin/user-details-page'
import { getUserID, deleteUser } from '../../api-utils.ts/users-util'

fixture`Brand Tests`
	.meta('TestRun', '1')
	.before(async ctx => {
		ctx.clientAuth = await adminClientSetup()
		ctx.buyerID = await createDefaultBuyer(ctx.clientAuth)
		const createdCatalog = await createDefaultCatalog(
			ctx.buyerID,
			ctx.clientAuth
		)
		ctx.catalogID = createdCatalog.ID
		ctx.catalogName = createdCatalog.Name
		const createdLocation = await createDefaultBuyerLocation(
			ctx.buyerID,
			ctx.clientAuth
		)
		ctx.locationID = createdLocation.Address.ID
		ctx.locationName = createdLocation.Address.AddressName
	})
	.beforeEach(async t => {
		await adminTestSetup()
	})
	.after(async ctx => {
		await deleteBuyerLocation(ctx.buyerID, ctx.locationID, ctx.clientAuth)
		await deleteCatalog(ctx.catalogID, ctx.buyerID, ctx.clientAuth)
		await deleteBuyer(ctx.buyerID, ctx.clientAuth)
	})
	.page(testConfig.adminAppUrl)

test.after(async () => {
	await deleteBuyerWithName(t.ctx.brandName, t.fixtureCtx.clientAuth)
})('Create Brand', async () => {
	await adminHeaderPage.selectAllBrands()
	await mainResourcePage.clickCreateButton()
	const brandName = await brandDetailsPage.createDefaultBrand()
	t.ctx.brandName = brandName
	await t.expect(await mainResourcePage.resourceExists(brandName)).ok()
})

//failing because of bug, create works but object is not shown until page is refreshed
//NEED to create JIRA ticket for this issue
test.after(async () => {
	await deleteCatalogWithName(
		t.ctx.createdCatalogName,
		t.fixtureCtx.buyerID,
		t.fixtureCtx.clientAuth
	)
})('Create Brand Catalog', async () => {
	await adminHeaderPage.selectBrandCatalogs()
	await minorResourcePage.selectParentResourceDropdown(t.fixtureCtx.buyerID)
	await minorResourcePage.clickCreateButton()
	const createdCatalogName = await catalogDetailsPage.createDefaultCatalog()
	t.ctx.createdCatalogName = createdCatalogName
	await t
		.expect(await minorResourcePage.resourceExists(createdCatalogName))
		.ok('Brand Catalog not found in resource list')
})

//failing because of https://four51.atlassian.net/browse/SEB-933
test.after(async () => {
	await deleteBuyerLocationWithName(
		t.ctx.createdLocationName,
		t.fixtureCtx.buyerID,
		t.fixtureCtx.clientAuth
	)
})('Create Brand Location', async () => {
	await adminHeaderPage.selectBrandLocations()
	await minorResourcePage.selectParentResourceDropdown(t.fixtureCtx.buyerID)
	await minorResourcePage.clickCreateButton()
	const createdLocationName = await locationDetailsPage.createDefaultLocation()
	t.ctx.createdLocationName = createdLocationName
	await t
		.expect(await minorResourcePage.resourceExists(createdLocationName))
		.ok('Brand Location not found in resource list')
})

test('Assign Brand Location to Brand Catalog', async t => {
	await adminHeaderPage.selectBrandLocations()
	await minorResourcePage.selectParentResourceDropdown(t.fixtureCtx.buyerID)
	await minorResourcePage.clickResource(t.fixtureCtx.locationID)
	await locationDetailsPage.assignCatalogToLocation(t.fixtureCtx.catalogName)
})

//https://four51.atlassian.net/browse/SEB-944
test.after(async t => {
	const createdUserID = await getUserID(
		t.ctx.createdUserEmail,
		t.fixtureCtx.buyerID,
		t.fixtureCtx.clientAuth
	)
	await deleteUser(
		createdUserID,
		t.fixtureCtx.buyerID,
		t.fixtureCtx.clientAuth
	)
})('Create And Assign Brand User To Location', async t => {
	await adminHeaderPage.selectBrandUsers()
	await minorResourcePage.selectParentResourceDropdown(t.fixtureCtx.buyerID)
	await minorResourcePage.clickCreateButton()
	const createdUserEmail = await userDetailsPage.createDefaultBrandUserWithLocation(
		t.fixtureCtx.locationName
	)
	t.ctx.createdUserEmail = createdUserEmail
	await t.expect(await minorResourcePage.resourceExists(createdUserEmail)).ok()
})
