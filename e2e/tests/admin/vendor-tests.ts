import { t } from 'testcafe'
import testConfig from '../../testConfig'
import { adminTestSetup, adminClientSetup } from '../../helpers/test-setup'
import adminHeaderPage from '../../pages/admin/admin-header-page'
import {
	getVendorID,
	deleteVendor,
	createVendor,
} from '../../api-utils.ts/vendor-util'
import {
	getVendorUserID,
	deleteVendorUser,
} from '../../api-utils.ts/users-util'
import {
	getWarehouseID,
	deleteWarehouse,
} from '../../api-utils.ts/warehouse-util'
import loadingHelper from '../../helpers/loading-helper'
import mainResourcePage from '../../pages/admin/main-resource-page'
import vendorDetailsPage from '../../pages/admin/vendor-details-page'
import minorResourcePage from '../../pages/admin/minor-resource-page'
import userDetailsPage from '../../pages/admin/user-details-page'
import warehouseDetailsPage from '../../pages/admin/warehouse-details-page'

fixture`Vendor Tests`
	.meta('TestRun', '1')
	.before(async ctx => {
		ctx.clientAuth = await adminClientSetup()
	})
	.page(testConfig.adminAppUrl)

test
	.before(async () => {
		await adminTestSetup()
	})
	.after(async t => {
		const vendorID = await getVendorID(
			t.fixtureCtx.vendorName,
			t.fixtureCtx.clientAuth
		)
		await deleteVendor(vendorID, t.fixtureCtx.clientAuth)
	})('Create Vendor', async t => {
	await adminHeaderPage.selectAllVendors()
	await mainResourcePage.clickCreateButton()
	const vendorName = await vendorDetailsPage.createDefaultVendor()
	t.ctx.vendorName = vendorName
	await t.expect(await mainResourcePage.resourceExists(vendorName)).ok()
})

test
	.before(async t => {
		await adminTestSetup()
		t.ctx.vendorID = await createVendor(t.fixtureCtx.clientAuth)
	})
	.after(async t => {
		const createdUserID = await getVendorUserID(
			t.ctx.createdUserEmail,
			t.ctx.vendorID,
			t.fixtureCtx.clientAuth
		)
		await deleteVendorUser(
			createdUserID,
			t.ctx.vendorID,
			t.fixtureCtx.clientAuth
		)
		await deleteVendor(t.ctx.vendorID, t.fixtureCtx.clientAuth)
	})('Create Vendor User', async t => {
	await adminHeaderPage.selectVendorUsers()
	await minorResourcePage.selectParentResourceDropdown(t.ctx.vendorID)
	await minorResourcePage.clickCreateButton()
	const createdUserEmail = await userDetailsPage.createDefaultUser()
	t.ctx.createdUserEmail = createdUserEmail
	await t.expect(await minorResourcePage.resourceExists(createdUserEmail)).ok()
})

test
	.before(async t => {
		await adminTestSetup()
		t.ctx.vendorID = await createVendor(t.fixtureCtx.clientAuth)
	})
	.after(async t => {
		const warehouseID = await getWarehouseID(
			t.ctx.warehouseName,
			t.ctx.vendorID,
			t.fixtureCtx.clientAuth
		)
		await deleteWarehouse(
			warehouseID,
			t.ctx.vendorID,
			t.fixtureCtx.clientAuth
		)
		await deleteVendor(t.ctx.vendorID, t.fixtureCtx.clientAuth)
	})('Create Vendor Warehouse', async () => {
	await adminHeaderPage.selectVendorWarehouses()
	await minorResourcePage.selectParentResourceDropdown(t.ctx.vendorID)
	await minorResourcePage.clickCreateButton()
	const warehouseName = await warehouseDetailsPage.createDefaultWarehouse()
	await loadingHelper.waitForTwoLoadingBars()
	t.ctx.warehouseName = warehouseName
	await t.expect(await minorResourcePage.resourceExists(warehouseName)).ok()
})
