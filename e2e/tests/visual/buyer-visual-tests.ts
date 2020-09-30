/* eslint-disable @typescript-eslint/ban-ts-ignore */
import Eyes from '@applitools/eyes-testcafe'
import { checkWindow, getBrowsers } from '../../helpers/eyes-helper'
import testConfig from '../../testConfig'

const eyes = new Eyes()
const appName = 'Headstart Buyer'

fixture`Headstart Buyer Visual Tests`
	.meta('TestRun', 'Visual')
	.afterEach(async () => {
		await eyes.close()
	})
	.after(async () => {
		await eyes.waitForResults()
	})
	.page(testConfig.buyerAppUrl)

test('Buyer Home Page', async t => {
	await t.maximizeWindow()
	await eyes.open({
		appName: appName,
		// @ts-ignore
		testName: t.testRun.test.name,
		// @ts-ignore
		browser: getBrowsers(),
		t,
		accessibilityValidation: { level: 'AA', guidelinesVersion: 'WCAG_2_0' },
	})
	// @ts-ignore
	await checkWindow(eyes, t.testRun.test.name)
})
