import { t } from 'testcafe'
import testConfig from '../../testConfig'
import {
	adminTestSetup,
	adminClientSetup,
	vendorTestSetup,
} from '../../helpers/test-setup'
import {
	createSupplier,
	deleteSupplier,
} from '../../api-utils.ts/supplier-util'
import {
	createDefaultSupplierAddress,
	deleteSupplierAddress,
} from '../../api-utils.ts/warehouse-util'
import {
	createDefaultSupplierUser,
	getSupplierUser,
	deleteSupplierUser,
	createDefaultSupplierUserWithoutRoles,
} from '../../api-utils.ts/supplier-users-util'
import adminHeaderPage from '../../pages/admin/admin-header-page'
import mainResourcePage from '../../pages/admin/main-resource-page'
import productDetailsPage from '../../pages/admin/product-details-page'
import {
	getProductID,
	deleteProduct,
	createDefaultProduct,
} from '../../api-utils.ts/product-util'
import { delay } from '../../helpers/wait-helper'
import { createDefaultBuyer, deleteBuyer } from '../../api-utils.ts/buyer-util'
import {
	createDefaultCatalog,
	deleteCatalog,
} from '../../api-utils.ts/catalog-util'
import {
	createDefaultBuyerLocation,
	deleteBuyerLocation,
} from '../../api-utils.ts/buyer-locations-util'
import { userClientAuth, vendorUserRoles } from '../../api-utils.ts/auth-util'
import headerPage from '../../pages/buyer/buyer-header-page'
import { scrollIntoView } from '../../helpers/element-helper'

fixture`Product Tests`
	.meta('TestRun', '1')
	.before(async ctx => {
		ctx.clientAuth = await adminClientSetup()
		ctx.supplierID = await createSupplier(ctx.clientAuth)
		ctx.supplierUserID = await createDefaultSupplierUser(
			ctx.supplierID,
			ctx.clientAuth
		)
		ctx.warehouseID = await createDefaultSupplierAddress(
			ctx.supplierID,
			ctx.clientAuth
		)
		const supplierUser = await getSupplierUser(
			ctx.supplierUserID,
			ctx.supplierID,
			ctx.clientAuth
		)
		ctx.supplierUserAuth = await userClientAuth(
			testConfig.adminAppClientID,
			supplierUser.Username,
			'Test123!',
			vendorUserRoles
		)
		ctx.productID = await createDefaultProduct(
			ctx.warehouseID,
			ctx.supplierUserAuth
		)
		ctx.buyerID = await createDefaultBuyer(ctx.clientAuth)
		const catalog = await createDefaultCatalog(ctx.buyerID, ctx.clientAuth)
		ctx.catalogID = catalog.ID
		const location = await createDefaultBuyerLocation(
			ctx.buyerID,
			ctx.clientAuth
		)
		ctx.locationID = location.Address.ID
		//wait 20 seconds to let everything get setup
		await delay(20000)
	})
	.after(async ctx => {
		await deleteProduct(ctx.productID, ctx.supplierUserAuth)
		await deleteCatalog(ctx.catalogID, ctx.buyerID, ctx.clientAuth)
		await deleteBuyerLocation(ctx.buyerID, ctx.locationID, ctx.clientAuth)
		await deleteBuyer(ctx.buyerID, ctx.clientAuth)
		await deleteSupplierAddress(
			ctx.warehouseID,
			ctx.supplierID,
			ctx.clientAuth
		)
		await deleteSupplierUser(
			ctx.supplierUserID,
			ctx.supplierID,
			ctx.clientAuth
		)
		await deleteSupplier(ctx.supplierID, ctx.clientAuth)
	})
	.page(testConfig.adminAppUrl)

//Product not being shown in UI after create, new to reload page to see
//failing because of https://four51.atlassian.net/browse/SEB-725
test
	.before(async t => {
		const vendorUser = await getSupplierUser(
			t.fixtureCtx.supplierUserID,
			t.fixtureCtx.supplierID,
			t.fixtureCtx.clientAuth
		)
		await vendorTestSetup(vendorUser.Username, 'Test123!')
	})
	.after(async () => {
		if (t.ctx.createdProductName != null) {
			const createdProductID = await getProductID(
				t.ctx.createdProductName,
				t.fixtureCtx.clientAuth
			)
			await deleteProduct(createdProductID, t.ctx.userAuth)
		}
	})('Create Product | 19215', async t => {
	await adminHeaderPage.selectAllProducts()
	await mainResourcePage.clickCreateNewStandardProduct()
	const createdProductName = await productDetailsPage.createDefaultProduct()
	t.ctx.createdProductName = createdProductName
	await t
		.expect(await mainResourcePage.resourceExists(createdProductName))
		.ok()
})

test.before(async () => {
	await adminTestSetup()
})('Assign Product to Catalog | 19976', async t => {
	await adminHeaderPage.selectAllProducts()
	await mainResourcePage.searchForResource(t.fixtureCtx.productID)
	await mainResourcePage.selectResource(t.fixtureCtx.productID)
	await productDetailsPage.clickBuyerVisibilityTab()
	await productDetailsPage.editBuyerVisibility(t.fixtureCtx.buyerID)
})

//Check that new product shows up on buyer side
