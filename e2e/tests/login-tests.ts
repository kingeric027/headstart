import * as OrderCloudSDK from 'ordercloud-javascript-sdk'
import { ClientFunction } from 'testcafe'
import loginPage from '../pages/login-page'
import testConfig from '../testConfig'
import headerPage from '../pages/header-page'
import {
	adminClientSetup,
	loginTestSetup,
	loginTestCleanup,
} from '../helpers/test-setup'
import adminHeaderPage from '../pages/admin/admin-header-page'

const getLocation = ClientFunction(() => document.location.href)

fixture`Log In Tests`
	.meta('TestRun', '1')
	.before(async ctx => {
		ctx.adminClientAuth = await adminClientSetup()
	})
	.page(testConfig.buyerAppUrl)

test
	.before(async t => {
		t.ctx.testUser = await loginTestSetup(t.fixtureCtx.adminClientAuth)
	})
	.after(async t => {
		await loginTestCleanup(
			t.ctx.testUser.ID,
			'0005',
			t.fixtureCtx.adminClientAuth
		)
	})('Buyer Log In And Out | 19700', async t => {
	const testUser: OrderCloudSDK.User = t.ctx.testUser
	await loginPage.login(testUser.Username, testUser.Password)
	await t.expect(getLocation()).contains('home')
	await headerPage.logout()
	await t.expect(getLocation()).contains('login')
	await t.expect(loginPage.submitButton.exists).ok()
})

test('Admin Log In And Out | 19966', async t => {
	await t.maximizeWindow()
	await loginPage.login(
		testConfig.adminSellerUsername,
		testConfig.adminSellerPassword
	)
	await t.expect(getLocation()).contains('home')
	await adminHeaderPage.logout()
	await t.expect(getLocation()).contains('login')
	await t.expect(loginPage.submitButton.exists).ok()
}).page(testConfig.adminAppUrl)
