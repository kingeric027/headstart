import { ClientFunction } from 'testcafe'
import testConfig from '../../testConfig'
import { adminTestSetup, adminClientSetup } from '../../helpers/test-setup'

fixture.skip`Brand Tests`
	.meta('TestRun', '1')
	.before(async ctx => {
		ctx.clientAuth = await adminClientSetup()
	})
	.page(testConfig.adminAppUrl)

test
	.before(async () => {
		await adminTestSetup()
	})
	.after(async () => {
		//Delete Brand
	})('Create Brand', async () => {})

test
	.before(async () => {
		await adminTestSetup()
		//Create Brand
	})
	.after(async () => {
		//Delete Catalog
		//Delete Brand
	})('Create Brand Catalog', async () => {})

test
	.before(async () => {
		await adminTestSetup()
		//Create Brand
	})
	.after(async () => {
		//Delete Location
		//Delte Brand
	})('Create Brand Location', async () => {})

//Assign brand location to brand catalog

test
	.before(async () => {
		await adminTestSetup()
		//Create Brand
		//Create Catalog
		//Create Location
	})
	.after(async () => {
		//Delete User
		//Delete Location
		//Delete Catalog
		//Delte Brand
	})('Create Brand User', async () => {})

//Assign brand user to brand location
