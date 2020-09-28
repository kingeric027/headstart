import { adminClientSetup, adminTestSetup } from '../../helpers/test-setup'
import testConfig from '../../testConfig'
import adminHeaderPage from '../../pages/admin/admin-header-page'
import mainResourcePage from '../../pages/admin/main-resource-page'
import vendorDetailsPage from '../../pages/admin/vendor-details-page'
import minorResourcePage from '../../pages/admin/minor-resource-page'
import userDetailsPage from '../../pages/admin/user-details-page'

fixture`Vendor Setup`
	.meta('TestRun', 'Setup')
	.before(async ctx => {
		ctx.clientAuth = await adminClientSetup()
	})
	.beforeEach(async t => {
		await adminTestSetup()
	})
	.page(testConfig.adminAppUrl)

test('Setup Create Vendor', async t => {
	await adminHeaderPage.selectAllVendors()
	await mainResourcePage.clickCreateButton()
	await vendorDetailsPage.createVendor(
		true,
		"Speedwagon's Foundation",
		'united states dollar',
		['Standard', 'Quote', 'Purchase Order'],
		'Linens',
		'mandated',
		['UnitedStates', 'Canada']
	)
})
