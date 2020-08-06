import { t } from 'testcafe'

export async function refreshPage() {
	await t.eval(() => location.reload(true))
}
