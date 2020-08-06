import * as OrderCloudSDK from 'ordercloud-javascript-sdk'
import randomString from '../helpers/random-string'
import testConfig from '../testConfig'

export async function getVendorID(supplierName: string, clientAuth: string) {
	const searchResponse = await OrderCloudSDK.Suppliers.List(
		{ search: supplierName, searchOn: 'Name' },
		{ accessToken: clientAuth }
	)

	const supplier = searchResponse.Items.find(x => x.Name === supplierName)

	if (supplier.Name.includes('AutomationVendor_')) return supplier.ID
}

export async function deleteVendor(supplierID: string, clientAuth: string) {
	await OrderCloudSDK.Suppliers.Delete(supplierID, { accessToken: clientAuth })
}

export async function createVendor(clientAuth: string) {
	const vendor: OrderCloudSDK.Supplier = {
		Name: `AutomationVendor_${randomString(5)}`,
		Active: true,
		xp: {
			ApiClientID: testConfig.automationClientID, //looks like each vendor gets their own client, for automation vendors we will use the same client
			CountriesServicing: ['US'],
			Currency: 'USD',
			ProductTypes: ['Standard'],
		},
	}

	const createdVendor = await OrderCloudSDK.Suppliers.Create(vendor, {
		accessToken: clientAuth,
	})

	return createdVendor.ID
}
