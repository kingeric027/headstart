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
} from '../../api-utils.ts/supplier-users-util'
import adminHeaderPage from '../../pages/admin/admin-header-page'
import mainResourcePage from '../../pages/admin/main-resource-page'
import productDetailsPage from '../../pages/admin/product-details-page'
import { getProductID, deleteProduct } from '../../api-utils.ts/product-util'

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
test
	.before(async t => {
		//wait 10 seconds for user roles to be updated after creating the user
		await t.wait(10000)
		const vendorUser = await getSupplierUser(
			t.fixtureCtx.supplierUserID,
			t.fixtureCtx.supplierID,
			t.fixtureCtx.clientAuth
		)
		console.log(vendorUser)
		await t.debug()
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
	})('Create Product', async t => {
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
		await adminTestSetup()
		//Create Vendor
		//Create Vendor User
		//Create Vendor Warehouse
		//Create Product
		//Create Brand + Catalog + Location + User
	})
	.after(async () => {
		//Delete Product
		//Delete Vendor
		//Delete Vendor User
		//Delete Vendor Warehouse
		//Delete Brand + Catalog + Location + User
	})('Assign Product', async () => {}) //as seller user

//Check that new product shows up on buyer side
