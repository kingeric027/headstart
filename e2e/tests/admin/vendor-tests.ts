import { t } from 'testcafe'
import testConfig from '../../testConfig'
import { adminTestSetup, adminClientSetup } from '../../helpers/test-setup'
import adminHeaderPage from '../../pages/admin/admin-header-page'
import { createSupplier } from '../../api-utils.ts/supplier-util'
import {
	getWarehouseID,
	deleteSupplierAddress,
} from '../../api-utils.ts/warehouse-util'
import mainResourcePage from '../../pages/admin/main-resource-page'
import vendorDetailsPage from '../../pages/admin/vendor-details-page'
import minorResourcePage from '../../pages/admin/minor-resource-page'
import userDetailsPage from '../../pages/admin/user-details-page'
import warehouseDetailsPage from '../../pages/admin/warehouse-details-page'
import {
	getSupplierUserID,
	deleteSupplierUser,
	createDefaultSupplierUser,
} from '../../api-utils.ts/supplier-users-util'
import {
	cleanupVendorWithName,
	cleanupVendorWithID,
} from '../../helpers/test-cleanup'

fixture`Vendor Tests`
	.meta('TestRun', '1')
	.before(async ctx => {
		ctx.clientAuth = await adminClientSetup()
		ctx.supplierID = await createSupplier(ctx.clientAuth)
		ctx.supplierUserID = await createDefaultSupplierUser(
			ctx.supplierID,
			ctx.clientAuth
		)
	})
	.beforeEach(async t => {
		await adminTestSetup()
	})
	.after(async ctx => {
		await deleteSupplierUser(
			ctx.supplierUserID,
			ctx.supplierID,
			ctx.clientAuth
		)
		await cleanupVendorWithID(ctx.supplierID, ctx.clientAuth)
	})
	.page(testConfig.adminAppUrl)

test.after(async t => {
	await cleanupVendorWithName(t.ctx.vendorName, t.fixtureCtx.clientAuth)
})('Create Vendor', async t => {
	await adminHeaderPage.selectAllVendors()
	await mainResourcePage.clickCreateButton()
	const vendorName = await vendorDetailsPage.createDefaultVendor()
	t.ctx.vendorName = vendorName
	await t.expect(await mainResourcePage.resourceExists(vendorName)).ok()
})

test.after(async t => {
	const createdUserID = await getSupplierUserID(
		t.ctx.createdUserEmail,
		t.fixtureCtx.supplierID,
		t.fixtureCtx.clientAuth
	)
	await deleteSupplierUser(
		createdUserID,
		t.fixtureCtx.supplierID,
		t.fixtureCtx.clientAuth
	)
})('Create Vendor User', async t => {
	await adminHeaderPage.selectVendorUsers()
	await minorResourcePage.selectParentResourceDropdown(t.fixtureCtx.supplierID)
	await minorResourcePage.clickCreateButton()
	const createdUserEmail = await userDetailsPage.createDefaultVendorUser()
	t.ctx.createdUserEmail = createdUserEmail
	await t.expect(await minorResourcePage.resourceExists(createdUserEmail)).ok()
})

test.after(async t => {
	const warehouseID = await getWarehouseID(
		t.ctx.warehouseName,
		t.fixtureCtx.supplierID,
		t.fixtureCtx.clientAuth
	)
	await deleteSupplierAddress(
		warehouseID,
		t.fixtureCtx.supplierID,
		t.fixtureCtx.clientAuth
	)
})('Create Vendor Warehouse', async t => {
	console.log(t.fixtureCtx.supplierID)
	await adminHeaderPage.selectVendorWarehouses()
	await minorResourcePage.selectParentResourceDropdown(t.fixtureCtx.supplierID)
	await minorResourcePage.clickCreateButton()
	const warehouseName = await warehouseDetailsPage.createDefaultWarehouse()
	t.ctx.warehouseName = warehouseName
	await t.expect(await minorResourcePage.resourceExists(warehouseName)).ok()
})

test('Assign Roles to Vendor User', async t => {
	await adminHeaderPage.selectVendorUsers()
	await minorResourcePage.selectParentResourceDropdown(t.fixtureCtx.supplierID)
	await minorResourcePage.clickResource(t.fixtureCtx.supplierUserID)
	await userDetailsPage.enableUserPermissions()
})
