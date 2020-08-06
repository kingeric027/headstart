import { Selector, t } from 'testcafe'

class LoadingHelper {
	loadingBar: Selector

	timeoutValue = 30000

	constructor() {
		this.loadingBar = Selector('.ng-progress-bar.-active')
	}

	async waitForLoadingBar() {
		await t
			.expect(this.loadingBar.exists)
			.notOk({ timeout: this.timeoutValue })
	}

	async waitForLoadingBarToExist() {
		await t.expect(this.loadingBar.exists).ok({ timeout: this.timeoutValue })
	}

	async waitForTwoLoadingBars() {
		await this.waitForLoadingBar()
		await this.waitForLoadingBarToExist()
		await this.waitForLoadingBar()
	}
}

export default new LoadingHelper()
