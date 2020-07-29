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

const getLocation = ClientFunction(() => document.location.href)

fixture`Log In Tests`
	.meta('TestRun', '1')
	.before(async ctx => {
		ctx.adminClientAuth = await adminClientSetup()
	})
	.page(testConfig.appUrl)

//failing due to https://four51.atlassian.net/browse/SEB-788
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
	})('Log In And Out With New User | 19700', async t => {
	const testUser: OrderCloudSDK.User = t.ctx.testUser
	await loginPage.login(testUser.Username, testUser.Password)
	await t.expect(getLocation()).contains('home')
	await headerPage.logout()
	await t.expect(getLocation()).contains('login')
	await t.expect(loginPage.submitButton.exists).ok()
})

test.skip('Log In And Out With Existing User | 19701', async t => {
	await t.maximizeWindow()
	await loginPage.login('erosen-buyer1', 'fails345')
	await t.expect(getLocation()).contains('home')
	await headerPage.logout()
	await t.expect(getLocation()).contains('login')
	await t.expect(loginPage.submitButton.exists).ok()
})
