import { HeadStartSDK } from '@ordercloud/headstart-sdk'
import * as OrderCloudSDK from 'ordercloud-javascript-sdk'
import { t } from 'testcafe'

export async function deleteProduct(productID: string, clientAuth: string) {
	await HeadStartSDK.Products.Delete(productID, clientAuth)
}

export async function getProductID(productName: string, clientAuth: string) {
	let searchResponse
	for (let i = 0; i < 5; i++) {
		searchResponse = await OrderCloudSDK.Products.List(
			{
				search: productName,
				searchOn: ['Name'],
			},
			{ accessToken: clientAuth }
		)
		if (searchResponse.Items.length > 0) {
			break
		} else {
			console.log('wait')
			await t.wait(5000)
		}
	}

	const product = searchResponse.Items.find(x => x.Name === productName)
	console.log(product)
	if (product.Name.includes('AutomationProduct_')) return product.ID
}
