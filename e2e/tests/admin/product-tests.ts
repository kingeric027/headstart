import { ClientFunction } from 'testcafe'
import testConfig from '../../testConfig'
import { adminTestSetup, adminClientSetup } from '../../helpers/test-setup'

fixture.skip`Product Tests`
	.meta('TestRun', '1')
	.before(async ctx => {
		ctx.clientAuth = await adminClientSetup()
	})
	.page(testConfig.adminAppUrl)

test
	.before(async () => {
		await adminTestSetup()
		//Create Vendor
		//Create Vendor User
		//Create Vendor Warehouse
	})
	.after(async () => {
		//Delete Product
		//Delete Vendor
		//Delete Vendor User
		//Delete Vendor Warehouse
	})('Create Product', async () => {}) //as vendor user

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
