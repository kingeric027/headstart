import * as OrderCloudSDK from 'ordercloud-javascript-sdk'

export async function getWarehouseID(
	warehouseName: string,
	vendorID: string,
	clientAuth: string
) {
	const searchResponse = await OrderCloudSDK.SupplierAddresses.List(
		vendorID,
		{ search: warehouseName, searchOn: 'AddressName' },
		{ accessToken: clientAuth }
	)

	const warehouse = searchResponse.Items.find(
		x => x.AddressName === warehouseName
	)

	if (warehouse.AddressName.includes('AutomationAddress_')) return warehouse.ID
}

export async function deleteWarehouse(
	warehouseID: string,
	vendorID: string,
	clientAuth: string
) {
	await OrderCloudSDK.SupplierAddresses.Delete(vendorID, warehouseID, {
		accessToken: clientAuth,
	})
}
