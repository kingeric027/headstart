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

fixture.skip`Product Tests`
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
		//wait 20 seconds to let everything get setup
		await delay(20000)
	})
	.after(async ctx => {
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
//Still WIP, running into a bug with authentication as a supplier user
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

test
	.before(async () => {
		//Create Product
		try {
			t.ctx.productID = await createDefaultProduct(
				t.fixtureCtx.warehouseID,
				t.fixtureCtx.supplierUserAuth
			)
		} catch (e) {
			console.log(e)
		}

		//Create Brand
		try {
			t.ctx.buyerID = await createDefaultBuyer(t.fixtureCtx.clientAuth)
		} catch (e) {
			console.log(e)
		}
		//Create Brand Catalog
		const catalog = await createDefaultCatalog(
			t.ctx.buyerID,
			t.fixtureCtx.clientAuth
		)
		t.ctx.catalogID = catalog.ID

		//Create Brand Location
		try {
			const location = await createDefaultBuyerLocation(
				t.ctx.buyerID,
				t.fixtureCtx.supplierUserAuth
			)
			t.ctx.locationID = location.Address.ID
		} catch (e) {
			console.log(e)
		}

		await adminTestSetup()
	})
	.after(async () => {
		//Delete Product
		await deleteProduct(t.ctx.productID, t.fixtureCtx.clientAuth)
		//Delete Brand Catalog
		await deleteCatalog(
			t.ctx.catalogID,
			t.ctx.buyerID,
			t.fixtureCtx.clientAuth
		)
		//Delete Brand Location
		await deleteBuyerLocation(
			t.ctx.buyerID,
			t.ctx.locationID,
			t.fixtureCtx.clientAuth
		)
		//Delete Brand
		await deleteBuyer(t.ctx.buyerID, t.fixtureCtx.clientAuth)
	})('Assign Product to Catalog | 19976', async t => {
	console.log(t.ctx.productID)
	console.log(t.ctx.buyerID)
	console.log(t.ctx.catalogID)
	await t.debug()
}) //as seller user (automation admin user)

//Check that new product shows up on buyer side
